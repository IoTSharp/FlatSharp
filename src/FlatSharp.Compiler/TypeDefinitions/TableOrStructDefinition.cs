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

namespace FlatSharp.Compiler
{
    using FlatSharp.Attributes;
    using FlatSharp.TypeModel;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;

    internal class TableOrStructDefinition : BaseSchemaMember
    {
        internal const string SerializerPropertyName = "Serializer";

        public TableOrStructDefinition(
            string name, 
            BaseSchemaMember parent) : base(name, parent)
        {
        }

        public List<FieldDefinition> Fields { get; set; } = new List<FieldDefinition>();

        public List<StructVectorDefinition> StructVectors { get; set; } = new List<StructVectorDefinition>();
        
        public bool IsTable { get; set; }

        public FlatBufferSchemaType SchemaType =>
            this.IsTable ? FlatBufferSchemaType.Table : FlatBufferSchemaType.Struct;

        public bool? NonVirtual { get; set; }

        public bool ObsoleteDefaultConstructor { get; set; }

        public string? FileIdentifier { get; set; }

        public FlatBufferDeserializationOption? RequestedSerializer { get; set; }

        protected override bool SupportsChildren => false;

        protected override void OnWriteCode(CodeWriter writer, CompileContext context)
        {
            this.AssignIndexes();

            string attribute = "[FlatBufferStruct]";

            if (this.IsTable)
            {
                if (string.IsNullOrEmpty(this.FileIdentifier))
                {
                    attribute = "[FlatBufferTable]";
                }
                else
                {
                    attribute = $"[FlatBufferTable({nameof(FlatBufferTableAttribute.FileIdentifier)} = \"{this.FileIdentifier}\")]";
                }
            }

            writer.AppendLine(attribute);
            writer.AppendLine("[System.Runtime.CompilerServices.CompilerGenerated]");
            writer.AppendLine($"public partial class {this.Name} : object");
            writer.AppendLine($"{{");

            using (writer.IncreaseIndent())
            {
                writer.AppendLine($"partial void OnInitialized({nameof(FlatSharpDeserializationContext)}? context);");

                // default ctor.
                if (this.ObsoleteDefaultConstructor)
                {
                    writer.AppendLine("[Obsolete]");
                }

                writer.AppendLine($"public {this.Name}()");
                using (writer.WithBlock())
                {
                    foreach (var field in this.Fields)
                    {
                        field.WriteDefaultConstructorLine(writer, context);
                    }

                    writer.AppendLine("this.OnInitialized(null);");
                }

                writer.AppendLine("#pragma warning disable CS8618"); // NULL FORGIVING
                writer.AppendLine($"protected {this.Name}({nameof(FlatSharpDeserializationContext)} context)");
                using (writer.WithBlock())
                {
                    writer.AppendLine("this.OnInitialized(context);");
                }
                writer.AppendLine("#pragma warning restore CS8618"); // NULL FORGIVING

                writer.AppendLine($"public {this.Name}({this.Name} source)");
                using (writer.WithBlock())
                {
                    foreach (var field in this.Fields)
                    {
                        field.WriteCopyConstructorLine(writer, "source", context);
                    }

                    writer.AppendLine("this.OnInitialized(null);");
                }

                foreach (var field in this.Fields)
                {
                    field.WriteField(writer, this, context);
                }

                foreach (var structVector in this.StructVectors)
                {
                    structVector.EmitStructVector(this, writer, context);
                }

                if (context.CompilePass >= CodeWritingPass.SerializerGeneration && this.RequestedSerializer is not null)
                {
                    // generate the serializer.
                    string serializer = this.GenerateSerializerForType(
                        context,
                        this.RequestedSerializer.Value);

                    writer.AppendLine($"public static ISerializer<{this.FullName}> {SerializerPropertyName} {{ get; }} = new {RoslynSerializerGenerator.GeneratedSerializerClassName}().AsISerializer();");
                    writer.AppendLine(string.Empty);
                    writer.AppendLine($"#region Serializer for {this.FullName}");
                    writer.AppendLine(serializer);
                    writer.AppendLine($"#endregion");
                }
            }

            writer.AppendLine($"}}");
        }

        public string ResolveTypeName(string fbsFieldType, CompileContext context)
        {
            if (context.TypeModelContainer.TryResolveFbsAlias(fbsFieldType, out ITypeModel? resolvedTypeModel))
            {
                fbsFieldType = resolvedTypeModel.ClrType.FullName ?? throw new InvalidOperationException("Full name was null");
            }
            else if (this.TryResolveName(fbsFieldType, out var node))
            {
                fbsFieldType = node.GlobalName;
            }

            return fbsFieldType;
        }

        private string GenerateSerializerForType(
            CompileContext context,
            FlatBufferDeserializationOption deserializationOption)
        {
            try
            {
                CSharpHelpers.ConvertProtectedInternalToProtected = false;

                Type? type = context.PreviousAssembly?.GetType(this.FullName);
                if (type is null)
                {
                    ErrorContext.Current.RegisterError($"Flatsharp failed to find expected type '{this.FullName}' in assembly.");
                    return string.Empty;
                }

                var options = new FlatBufferSerializerOptions(deserializationOption);
                var generator = new RoslynSerializerGenerator(options, context.TypeModelContainer);

                MethodInfo method = generator.GetType()
                                             .GetMethod(nameof(RoslynSerializerGenerator.GenerateCSharp), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)!
                                             .MakeGenericMethod(type);

                try
                {
                    string code = (string)method.Invoke(generator, new[] { "private" })!;
                    return code;
                }
                catch (TargetInvocationException ex)
                {
                    ExceptionDispatchInfo.Capture(ex.InnerException!).Throw();
                    throw;
                }
            }
            finally
            {
                CSharpHelpers.ConvertProtectedInternalToProtected = true;
            }
        }

        private void AssignIndexes()
        {
            ErrorContext.Current.WithScope(
                this.Name,
                () =>
                {
                    if (this.Fields.Any(field => field.IsIndexSetManually))
                    {
                        if (!this.IsTable)
                        {
                            ErrorContext.Current.RegisterError("Structure fields may not have 'id' attribute set.");
                        }

                        if (!this.Fields.TrueForAll(field => field.IsIndexSetManually))
                        {
                            ErrorContext.Current.RegisterError("All or none fields should have 'id' attribute set.");
                        }

                        return;
                    }

                    int nextIndex = 0;
                    foreach (var field in this.Fields)
                    {
                        field.Index = nextIndex;

                        nextIndex++;

                        if (this.TryResolveName(field.FbsFieldType, out var typeDef) && typeDef is UnionDefinition)
                        {
                            // Unions are double-wide.
                            nextIndex++;
                        }
                    }
                });
        }
    }
}
