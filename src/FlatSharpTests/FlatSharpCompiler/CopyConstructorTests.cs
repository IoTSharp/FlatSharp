﻿/*
 * Copyright 2021 James Courtney
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

#if NET5_0 || NETCOREAPP3_1

    [TestClass]
    public class CopyConstructorTests
    {
        [TestMethod]
        public void CopyConstructorsTest()
        {
            string schema = $@"
namespace CopyConstructorTest;

union Union {{ OuterTable, InnerTable, OuterStruct, InnerStruct }} // Optionally add more tables.

table OuterTable ({MetadataKeys.SerializerKind}: ""Greedy"") {{
  A:string;

  B:byte;
  C:ubyte;
  D:int16;
  E:uint16;
  F:int32;
  G:uint32;
  H:int64;
  I:uint64;
  
  IntVector_List:[int] ({MetadataKeys.VectorKind}:""IList"");
  IntVector_RoList:[int] ({MetadataKeys.VectorKind}:""IReadOnlyList"");
  IntVector_Array:[int] ({MetadataKeys.VectorKind}:""Array"");
  
  TableVector_List:[InnerTable] ({MetadataKeys.VectorKind}:""IList"");
  TableVector_RoList:[InnerTable] ({MetadataKeys.VectorKind}:""IReadOnlyList"");
  TableVector_Indexed:[InnerTable] ({MetadataKeys.VectorKind}:""IIndexedVector"");
  TableVector_Array:[InnerTable] ({MetadataKeys.VectorKindLegacy}:""Array"");

  ByteVector:[ubyte] ({MetadataKeys.VectorKind}:""Memory"");
  ByteVector_RO:[ubyte] ({MetadataKeys.VectorKind}:""ReadOnlyMemory"");
  Union:Union;
}}

struct OuterStruct {{
    Value:int;
    InnerStruct:InnerStruct;
}}

struct InnerStruct {{
    LongValue:int64;
}}

table InnerTable {{
  Name:string ({MetadataKeys.Key});
  OuterStruct:OuterStruct;
}}

";
            OuterTable original = new OuterTable
            {
                A = "string",
                B = 1,
                C = 2,
                D = 3,
                E = 4,
                F = 5,
                G = 6,
                H = 7,
                I = 8,

                ByteVector = new byte[] { 1, 2, 3, }.AsMemory(),
                ByteVector_RO = new byte[] { 4, 5, 6, }.AsMemory(),

                IntVector_Array = new[] { 7, 8, 9, },
                IntVector_List = new[] { 10, 11, 12, }.ToList(),
                IntVector_RoList = new[] { 13, 14, 15 }.ToList(),

                TableVector_Array = CreateInner("Rocket", "Molly", "Jingle"),
                TableVector_Indexed = new IndexedVector<string, InnerTable>(CreateInner("Pudge", "Sunshine", "Gypsy"), false),
                TableVector_List = CreateInner("Finnegan", "Daisy"),
                TableVector_RoList = CreateInner("Gordita", "Lunchbox"),

                Union = new FlatBufferUnion<OuterTable, InnerTable, OuterStruct, InnerStruct>(new OuterStruct())
            };

            byte[] data = new byte[FlatBufferSerializer.Default.GetMaxSize(original)];
            int bytesWritten = FlatBufferSerializer.Default.Serialize(original, data);

            Assembly asm = FlatSharpCompiler.CompileAndLoadAssembly(schema, new());

            Type outerTableType = asm.GetType("CopyConstructorTest.OuterTable");
            dynamic serializer = outerTableType.GetProperty("Serializer", BindingFlags.Public | BindingFlags.Static).GetValue(null);
            object parsedObj = serializer.Parse(new ArrayInputBuffer(data));
            dynamic parsed = parsedObj;
            dynamic copied = Activator.CreateInstance(outerTableType, (object)parsed);
            //dynamic copied = new CopyConstructorTest.OuterTable((CopyConstructorTest.OuterTable)parsedObj);

            // Strings can be copied by reference since they are immutable.
            Assert.AreEqual(original.A, copied.A);
            Assert.AreEqual(original.A, parsed.A);

            Assert.AreEqual(original.B, copied.B);
            Assert.AreEqual(original.C, copied.C);
            Assert.AreEqual(original.D, copied.D);
            Assert.AreEqual(original.E, copied.E);
            Assert.AreEqual(original.F, copied.F);
            Assert.AreEqual(original.G, copied.G);
            Assert.AreEqual(original.H, copied.H);
            Assert.AreEqual(original.I, copied.I);

            Assert.AreEqual((byte)3, original.Union.Discriminator);
            Assert.AreEqual((byte)3, parsed.Union.Discriminator);
            Assert.AreEqual((byte)3, copied.Union.Discriminator);
            Assert.AreEqual("CopyConstructorTest.OuterStruct", copied.Union.Item3.GetType().FullName);
            Assert.AreNotEqual("CopyConstructorTest.OuterStruct", parsed.Union.Item3.GetType().FullName);
            Assert.AreNotSame(parsed.Union, copied.Union);
            Assert.AreNotSame(parsed.Union.Item3, copied.Union.Item3);

            Memory<byte>? mem = copied.ByteVector;
            Memory<byte>? pMem = parsed.ByteVector;
            Assert.IsTrue(original.ByteVector.Value.Span.SequenceEqual(mem.Value.Span));
            Assert.IsFalse(mem.Value.Span.Overlaps(pMem.Value.Span));

            ReadOnlyMemory<byte>? roMem = copied.ByteVector_RO;
            ReadOnlyMemory<byte>? pRoMem = parsed.ByteVector_RO;
            Assert.IsTrue(original.ByteVector_RO.Value.Span.SequenceEqual(roMem.Value.Span));
            Assert.IsFalse(roMem.Value.Span.Overlaps(pRoMem.Value.Span));

            // array of table
            {
                int count = original.TableVector_Array.Length;
                Assert.AreNotSame(parsed.TableVector_Array, copied.TableVector_Array);
                for (int i = 0; i < count; ++i)
                {
                    var p = parsed.TableVector_Array[i];
                    var c = copied.TableVector_Array[i];
                    DeepCompareInnerTable(original.TableVector_Array[i], p, c);
                }
            }

            // list of table.
            {
                Assert.IsFalse(object.ReferenceEquals(parsed.TableVector_List, copied.TableVector_List));

                IEnumerable<object> parsedEnum = AsObjectEnumerable(parsed.TableVector_List);
                IEnumerable<object> copiedEnum = AsObjectEnumerable(copied.TableVector_List);

                foreach (var ((p, c), o) in parsedEnum.Zip(copiedEnum).Zip(original.TableVector_List))
                {
                    DeepCompareInnerTable(o, p, c);
                }
            }

            // read only list of table.
            {
                Assert.IsFalse(object.ReferenceEquals(parsed.TableVector_RoList, copied.TableVector_RoList));

                IEnumerable<object> parsedEnum = AsObjectEnumerable(parsed.TableVector_RoList);
                IEnumerable<object> copiedEnum = AsObjectEnumerable(copied.TableVector_RoList);

                foreach (var ((p, c), o) in parsedEnum.Zip(copiedEnum).Zip(original.TableVector_RoList))
                {
                    DeepCompareInnerTable(o, p, c);
                }
            }

            // indexed vector of table.
            {
                Assert.IsFalse(object.ReferenceEquals(parsed.TableVector_Indexed, copied.TableVector_Indexed));
                foreach (var kvp in original.TableVector_Indexed)
                {
                    string key = kvp.Key;
                    InnerTable? value = kvp.Value;

                    var parsedValue = parsed.TableVector_Indexed[key];
                    var copiedValue = copied.TableVector_Indexed[key];

                    Assert.IsNotNull(parsedValue);
                    Assert.IsNotNull(copiedValue);

                    DeepCompareInnerTable(value, parsedValue, copiedValue);
                }
            }
        }

        private InnerTable[] CreateInner(params string[] names)
        {
            InnerTable[] table = new InnerTable[names.Length];
            for (int i = 0; i < names.Length; ++i)
            {
                table[i] = new InnerTable 
                { 
                    Name = names[i],
                    OuterStruct = new OuterStruct
                    {
                        Value = 1,
                        InnerStruct = new InnerStruct
                        {
                            LongValue = 2,
                        },
                    }
                };
            }

            return table;
        }

        private static void DeepCompareInnerTable(InnerTable a, dynamic p, dynamic c)
        {
            Assert.AreNotSame(p, c);

            Assert.AreNotEqual("CopyConstructorTest.InnerTable", (string)p.GetType().FullName);
            Assert.AreEqual("CopyConstructorTest.InnerTable", (string)c.GetType().FullName);

            Assert.AreEqual(a.Name, p.Name);
            Assert.AreEqual(a.Name, c.Name);

            var pOuter = p.OuterStruct;
            var cOuter = c.OuterStruct;

            Assert.AreNotSame(pOuter, cOuter);
            Assert.AreNotEqual("CopyConstructorTest.OuterStruct", (string)pOuter.GetType().FullName);
            Assert.AreEqual("CopyConstructorTest.OuterStruct", (string)cOuter.GetType().FullName);

            Assert.AreEqual(a.OuterStruct.Value, pOuter.Value);
            Assert.AreEqual(a.OuterStruct.Value, cOuter.Value);

            var pInner = pOuter.InnerStruct;
            var cInner = cOuter.InnerStruct;

            Assert.AreNotSame(pInner, cInner);
            Assert.AreNotEqual("CopyConstructorTest.InnerStruct", (string)pInner.GetType().FullName);
            Assert.AreEqual("CopyConstructorTest.InnerStruct", (string)cInner.GetType().FullName);

            Assert.AreEqual(a.OuterStruct.InnerStruct.LongValue, pInner.LongValue);
            Assert.AreEqual(a.OuterStruct.InnerStruct.LongValue, cInner.LongValue);
        }

        [FlatBufferTable]
        public class OuterTable
        {
            [FlatBufferItem(0)]
            public string? A { get; set; }

            [FlatBufferItem(1)]
            public sbyte B { get; set; }

            [FlatBufferItem(2)]
            public byte C { get; set; }

            [FlatBufferItem(3)]
            public short D { get; set; }

            [FlatBufferItem(4)]
            public ushort E { get; set; }

            [FlatBufferItem(5)]
            public int F { get; set; }

            [FlatBufferItem(6)]
            public uint G { get; set; }

            [FlatBufferItem(7)]
            public long H { get; set; }

            [FlatBufferItem(8)]
            public ulong I { get; set; }

            [FlatBufferItem(9)]
            public IList<int>? IntVector_List { get; set; }

            [FlatBufferItem(10)]
            public IReadOnlyList<int>? IntVector_RoList { get; set; }

            [FlatBufferItem(11)]
            public int[]? IntVector_Array { get; set; }

            [FlatBufferItem(12)]
            public IList<InnerTable>? TableVector_List { get; set; }

            [FlatBufferItem(13)]
            public IReadOnlyList<InnerTable>? TableVector_RoList { get; set; }

            [FlatBufferItem(14)]
            public IIndexedVector<string, InnerTable>? TableVector_Indexed { get; set; }

            [FlatBufferItem(15)]
            public InnerTable[]? TableVector_Array { get; set; }

            [FlatBufferItem(16)]
            public Memory<byte>? ByteVector { get; set; }

            [FlatBufferItem(17)]
            public ReadOnlyMemory<byte>? ByteVector_RO { get; set; }

            [FlatBufferItem(18)]
            public FlatBufferUnion<OuterTable, InnerTable, OuterStruct, InnerStruct>? Union { get; set; }
        }

        [FlatBufferTable]
        public class InnerTable
        {
            [FlatBufferItem(0, Key = true)]
            public string? Name { get; set; }

            [FlatBufferItem(1)]
            public OuterStruct? OuterStruct { get; set; }
        }

        [FlatBufferStruct]
        public class OuterStruct
        {
            [FlatBufferItem(0)]
            public int Value { get; set; }

            [FlatBufferItem(1)]
            public InnerStruct InnerStruct { get; set; }
        }

        [FlatBufferStruct]
        public class InnerStruct
        {
            [FlatBufferItem(0)]
            public long LongValue { get; set; }
        }

        private static IEnumerable<object> AsObjectEnumerable(dynamic d)
        {
            System.Collections.IEnumerable enumerator = d;
            foreach (var item in enumerator)
            {
                yield return item;
            }
        }
    }

#endif
}