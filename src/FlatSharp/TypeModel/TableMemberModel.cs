﻿/*
 * Copyright 2018 James Courtney
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
 
 namespace FlatSharp.TypeModel
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    /// Describes a member of a FlatBuffer table.
    /// </summary>
    public class TableMemberModel : ItemMemberModel
    {
        public TableMemberModel(
            ITypeModel propertyModel, 
            PropertyInfo propertyInfo, 
            ushort index, 
            object? defaultValue,
            bool isSortedVector,
            bool isKey,
            bool isDeprecated) : base(propertyModel, propertyInfo, index)
        {
            this.DefaultValue = defaultValue;
            this.IsSortedVector = isSortedVector;
            this.IsKey = isKey;
            this.IsDeprecated = isDeprecated;

            if (!propertyModel.IsValidTableMember)
            {
                throw new InvalidFlatBufferDefinitionException($"Table property {propertyInfo.Name} with type {propertyInfo.PropertyType.Name} cannot be part of a flatbuffer table.");
            }

            if (this.DefaultValue is not null && !propertyModel.ValidateDefaultValue(this.DefaultValue))
            {
                throw new InvalidFlatBufferDefinitionException($"Table property {propertyInfo.Name} declared default value of type {propertyModel.ClrType.Name}, but the value was of type {defaultValue?.GetType()?.Name}. Please ensure that the property is allowed to have a default value and that the types match.");
            }
        }
        
        /// <summary>
        /// The default value of the table member.
        /// </summary>
        public object? DefaultValue { get; }

        /// <summary>
        /// Indicates if the member vector should be sorted before serializing.
        /// </summary>
        public bool IsSortedVector { get; }

        /// <summary>
        /// Indicates that this property is the key for the table.
        /// </summary>
        public bool IsKey { get; }

        /// <summary>
        /// Indicates that this property is deprecated and serializers need not be generated for it.
        /// </summary>
        public bool IsDeprecated { get; }

        /// <summary>
        /// Returns a C# literal that is equal to the default value.
        /// </summary>
        public string DefaultValueLiteral => this.ItemTypeModel.FormatDefaultValueAsLiteral(this.DefaultValue);

        public override string CreateReadItemBody(string parseItemMethodName, string bufferVariableName, string offsetVariableName, string vtableLocationVariableName, string vtableMaxIndexVariableName)
        {
            if (this.ItemTypeModel.PhysicalLayout.Length == 1)
            {
                return this.CreateSingleWidthReadItemBody(parseItemMethodName, bufferVariableName, offsetVariableName, vtableLocationVariableName, vtableMaxIndexVariableName);
            }
            else
            {
                return this.CreateWideReadItemBody(parseItemMethodName, bufferVariableName, offsetVariableName, vtableLocationVariableName, vtableMaxIndexVariableName);
            }
        }

        private string CreateSingleWidthReadItemBody(string parseItemMethodName, string bufferVariableName, string offsetVariableName, string vtableLocationVariableName, string vtableMaxIndexVariableName)
        {
            return $@"
                if ({this.Index} > {vtableMaxIndexVariableName})
                {{
                    return {this.DefaultValueLiteral};
                }}

                ushort relativeOffset = buffer.ReadUShort({vtableLocationVariableName} + {4 + (2 * this.Index)});
                if (relativeOffset == 0)
                {{
                    return {this.DefaultValueLiteral};
                }}

                int absoluteLocation = {offsetVariableName} + relativeOffset;
                return {parseItemMethodName}({bufferVariableName}, absoluteLocation);";
        }

        private string CreateWideReadItemBody(string parseItemMethodName, string bufferVariableName, string offsetVariableName, string vtableLocationVariableName, string vtableMaxIndexVariableName)
        {
            int items = this.ItemTypeModel.PhysicalLayout.Length;

            List<string> relativeOffsets = new();
            List<string> absoluteLocations = new();

            for (int i = 0; i < items; ++i)
            {
                int idx = this.Index + i;

                relativeOffsets.Add($@"
                ushort relativeOffset{i} = buffer.ReadUShort({vtableLocationVariableName} + {4 + (2 * idx)});
                if (relativeOffset{i} == 0)
                {{
                    return {this.DefaultValueLiteral};
                }}
                ");

                absoluteLocations.Add($"relativeOffset{i} + {offsetVariableName}");
            }

            return $@"
                if ({this.Index + items - 1} > {vtableMaxIndexVariableName})
                {{
                    return {this.DefaultValueLiteral};
                }}

                {string.Join("\r\n", relativeOffsets)}

                var absoluteLocations = ({string.Join(", ", absoluteLocations)});
                return {parseItemMethodName}({bufferVariableName}, ref absoluteLocations);";
        }
    }
}
