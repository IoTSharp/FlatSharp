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
    using System.Linq;
    using System.Reflection;
    using FlatSharp;
    using FlatSharp.Compiler;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class NullableAnnotationTests
    {
        [TestMethod]
        public void NullableAnnotations()
        {
            string schema = @"
            namespace NullableAnnotationTests;

            table Table {
                foo:Foo;
                defaultInt:int32 = 3;
                str:string;
                nullableInt:int32 = null;
                arrayVector:[int32] (vectortype:""Array"");
                memoryVector:[ubyte] (vectorType:""Memory"");
                listVector:[int32] (vectorType:""IList"");
                nestedTable:InnerTable;
            }

            table InnerTable { str:string; }

            struct Foo {
              id:ulong;
              count:short;
              prefix:byte;
              length:uint;
            }";

            Assembly asm = FlatSharpCompiler.CompileAndLoadAssembly(
                schema, 
                new());
        }
    }
}
