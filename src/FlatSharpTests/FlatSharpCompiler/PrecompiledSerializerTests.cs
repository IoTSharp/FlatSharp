﻿/*
 * Copyright 2020 James Courtney
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace FlatSharpTests.Compiler
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FlatSharp;
    using FlatSharp.Attributes;
    using FlatSharp.Compiler;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class PrecompiledSerializerTests
    {
        [TestMethod]
        public void MonsterTest()
        {
            // https://github.com/google/flatbuffers/blob/master/samples/monster.fbs
            string schema = $@"
namespace MyGame;
enum Color:byte {{ Red = 0, Green, Blue = 2 }}

union Equipment {{ Weapon, Vec3 }} // Optionally add more tables.

struct Vec3 {{
  x:float;
  y:float;
  z:float;
}}

table Monster ({MetadataKeys.SerializerKind}:""greedymutable"") {{
  pos:Vec3;
  mana:short = 150;
  hp:short = 100;
  name:string;
  friendly:bool = false ({MetadataKeys.Deprecated});
  inventory:[ubyte];
  color:Color = Blue;
  weapons:[Weapon];
  equipped:Equipment;
  path:[Vec3];
}}

table Weapon ({MetadataKeys.SerializerKind}:lazy) {{
  name:string;
  damage:short;
}}

root_type Monster;"; 

            Assembly asm = FlatSharpCompiler.CompileAndLoadAssembly(schema, new());

            Type weaponType = asm.GetType("MyGame.Weapon");
            Type monsterType = asm.GetTypes().Single(x => x.FullName == "MyGame.Monster");
            dynamic serializer = monsterType.GetProperty("Serializer", BindingFlags.Static | BindingFlags.Public).GetValue(null);

            object monster = Activator.CreateInstance(monsterType);
            dynamic dMonster = monster;

            Type vecType = asm.GetTypes().Single(x => x.FullName == "MyGame.Vec3");
            object vec = Activator.CreateInstance(vecType);
            dynamic dVec = vec;

            Assert.AreEqual((short)150, dMonster.mana);
            Assert.AreEqual((short)100, dMonster.hp);
            Assert.IsFalse(dMonster.friendly);
            Assert.AreEqual("Blue", dMonster.color.ToString());
            Assert.IsNull(dMonster.pos);

            Assert.AreEqual(typeof(IList<byte>), monsterType.GetProperty("inventory").PropertyType);
            Assert.AreEqual(typeof(IList<>).MakeGenericType(vecType), monsterType.GetProperty("path").PropertyType);
            Assert.AreEqual(typeof(IList<>).MakeGenericType(weaponType), monsterType.GetProperty("weapons").PropertyType);
            Assert.IsTrue(typeof(FlatBufferUnion<,>).MakeGenericType(weaponType, vecType).IsAssignableFrom(monsterType.GetProperty("equipped").PropertyType));
            Assert.AreEqual(typeof(string), monsterType.GetProperty("name").PropertyType);
            Assert.IsTrue(monsterType.GetProperty("friendly").GetCustomAttribute<FlatBufferItemAttribute>().Deprecated);

            byte[] data = new byte[1024];

            var compiled = CompilerTestHelpers.CompilerTestSerializer.Compile(monster);
            compiled.Write(data, monster);
            dynamic parsedMonster = compiled.Parse(data);

            Assert.AreEqual("Blue", parsedMonster.color.ToString());
        }

        [TestMethod]
        public void FlagsOptions_Greedy()
        {
            this.TestFlags(FlatBufferDeserializationOption.Greedy, $"{MetadataKeys.SerializerKind}:{nameof(FlatBufferDeserializationOption.Greedy)}");
        }

        [TestMethod]
        public void FlagsOptions_MutableGreedy()
        {
            this.TestFlags(FlatBufferDeserializationOption.GreedyMutable, $"{MetadataKeys.SerializerKind}:{nameof(FlatBufferDeserializationOption.GreedyMutable)}");
        }

        [TestMethod]
        public void FlagsOptions_Default()
        {
            this.TestFlags(FlatBufferDeserializationOption.Default, $"{MetadataKeys.SerializerKind}:{nameof(FlatBufferDeserializationOption.Default)}");
        }

        [TestMethod]
        public void FlagsOptions_Default_Implicit()
        {
            this.TestFlags(FlatBufferDeserializationOption.Default, $"{MetadataKeys.SerializerKind}");
        }

        [TestMethod]
        public void FlagsOptions_Lazy()
        {
            this.TestFlags(FlatBufferDeserializationOption.Lazy, $"{MetadataKeys.SerializerKind}:{nameof(FlatBufferDeserializationOption.Lazy)}");
        }

        [TestMethod]
        public void FlagsOptions_MutableVectorCache()
        {
            this.TestFlags(FlatBufferDeserializationOption.VectorCacheMutable, $"{MetadataKeys.SerializerKind}:{nameof(FlatBufferDeserializationOption.VectorCacheMutable)}");
        }

        [TestMethod]
        public void FlagsOptions_VectorCache()
        {
            this.TestFlags(FlatBufferDeserializationOption.VectorCache, $"{MetadataKeys.SerializerKind}:{nameof(FlatBufferDeserializationOption.VectorCache)}");
        }

        [TestMethod]
        public void FlagsOptions_PropertyCache()
        {
            this.TestFlags(FlatBufferDeserializationOption.PropertyCache, $"{MetadataKeys.SerializerKind}:{nameof(FlatBufferDeserializationOption.PropertyCache)}");
        }

        [TestMethod]
        public void FlagsOptions_Invalid()
        {
            Assert.ThrowsException<InvalidFbsFileException>(() => this.TestFlags(default, $"{MetadataKeys.SerializerKind}:banana"));
            Assert.ThrowsException<InvalidFbsFileException>(() => this.TestFlags(default, $"{MetadataKeys.SerializerKind}:\"banana\""));
            Assert.ThrowsException<InvalidFbsFileException>(() => this.TestFlags(default, $"{MetadataKeys.SerializerKind}:\"greedy|banana\""));
            Assert.ThrowsException<InvalidFbsFileException>(() => this.TestFlags(default, $"{MetadataKeys.SerializerKind}:\"greedy|mutablegreedy\""));
        }

        private void TestFlags(FlatBufferDeserializationOption expectedFlags, string metadata)
        {
            string schema = $"namespace Test; table FooTable ({metadata}) {{ foo:string; bar:string; }}";
            Assembly asm = FlatSharpCompiler.CompileAndLoadAssembly(schema, new());

            Type type = asm.GetType("Test.FooTable");
            Assert.IsNotNull(type);

            Type serializerType = type.GetNestedType(RoslynSerializerGenerator.GeneratedSerializerClassName, BindingFlags.NonPublic | BindingFlags.Public);
            
            Assert.IsNotNull(serializerType);
            Assert.IsTrue(serializerType.IsNested);
            Assert.IsTrue(serializerType.IsNestedPrivate);

            var attribute = serializerType.GetCustomAttribute<FlatSharpGeneratedSerializerAttribute>();
            Assert.IsNotNull(attribute);
            Assert.AreEqual(expectedFlags, attribute.DeserializationOption);
        }
    }
}

