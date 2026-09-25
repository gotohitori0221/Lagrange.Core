using Lagrange.Proto.Generator.Entity;
using Lagrange.Proto.Generator.Utility;
using Lagrange.Proto.Generator.Utility.Extension;
using Lagrange.Proto.Serialization;
using Microsoft.CodeAnalysis;

namespace Lagrange.Proto.Generator;

public partial class ProtoSourceGenerator
{
    private partial class Emitter
    {
        private const string ConstantVarName = "constant";
        private const string LengthVarName = "length";

        private const string ProtoHelperTypeRef = "global::Lagrange.Proto.Utility.ProtoHelper";
        private const string ProtoResolvableUtilityTypeRef = "global::Lagrange.Proto.Primitives.ProtoResolvableUtility";

        private const string GetVarIntLengthMethodRef = $"{ProtoHelperTypeRef}.GetVarIntLength";
        private const string CountStringMethodRef = $"{ProtoHelperTypeRef}.CountString";
        private const string CountBytesMethodRef = $"{ProtoHelperTypeRef}.CountBytes";
        private const string MeasureMethodRef = $"{ProtoResolvableUtilityTypeRef}.Measure";

        private void EmitMeasureMethod(SourceWriter source)
        {
            source.WriteLine($"public static int MeasureHandler({_fullQualifiedName} {ObjectVarName})");
            source.WriteLine("{");
            source.Indentation++;

            EmitConstant(source);
            source.WriteLine($"int {LengthVarName} = {ConstantVarName};");
            source.WriteLine();

            foreach (var kv in parser.Fields)
            {
                int field = kv.Key;
                var info = kv.Value;

                EmitLengthStatement(source, field, info);
            }

            source.WriteLine();
            source.WriteLine($"return {LengthVarName};");

            source.Indentation--;
            source.WriteLine("}");
        }

        private void EmitConstant(SourceWriter source)
        {
            if (parser.IgnoreDefaultFields)
            {
                source.WriteLine($"const int {ConstantVarName} = 0;");
                return;
            }

            int constant = 0;
            foreach (var kv in parser.Fields)
            {
                int field = kv.Key;
                var info = kv.Value;

                var tag = ProtoHelper.EncodeVarInt((uint)field << 3 | (byte)info.WireType);
                if (info.TypeSymbol.IsValueType && !info.TypeSymbol.IsNullable()) constant += tag.Length;
            }

            source.WriteLine($"const int {ConstantVarName} = {constant};");
        }

        private void EmitLengthStatement(SourceWriter source, int field, ProtoFieldInfo info)
        {
            uint tag = (uint)field << 3 | (byte)info.WireType;
            var encodedTag = ProtoHelper.EncodeVarInt(tag);

            string memberName;
            if (info.TypeSymbol.IsValueType && info.TypeSymbol.IsNullable())
            {
                memberName = $"{ObjectVarName}.{info.Symbol.Name}.Value";
            }
            else if (info.ExtraTypeInfo.Count != 0)
            {
                memberName = $"{ObjectVarName}";
            }
            else
            {
                memberName = $"{ObjectVarName}.{info.Symbol.Name}";
            }

            string lengthMember = GenerateLengthMember(field, info, memberName);
            string expression;

            if (parser.IgnoreDefaultFields)
            {
                expression = info.TypeSymbol.IsValueType
                    ? GenerateIfNotDefaultExpression(info.TypeSymbol, $"{ObjectVarName}.{info.Symbol.Name}", $"{encodedTag.Length} + {lengthMember}", "0")
                    : GenerateShouldSerializeExpression(tag, $"{encodedTag.Length} + {lengthMember}", "0"); 
            }
            else
            {
                if (info.TypeSymbol.IsValueType)
                {
                    expression = info.TypeSymbol.IsNullable()
                        ? GenerateIfNotNullExpression($"{ObjectVarName}.{info.Symbol.Name}", $"{encodedTag.Length} + {lengthMember}", "0")
                        : lengthMember;
                }
                else
                {
                    expression = GenerateShouldSerializeExpression(tag, $"{encodedTag.Length} + {lengthMember}", "0");
                }
            }

            source.WriteLine($"{LengthVarName} += {expression};");
        }

        private static string GenerateLengthMember(int field, ProtoFieldInfo info, string memberName)
        {
            return info.WireType switch
            {
                WireType.VarInt when info.TypeSymbol.IsIntegerType() => $"{GetVarIntLengthMethodRef}({memberName})",
                WireType.Fixed32 => "4",
                WireType.Fixed64 => "8",
                WireType.LengthDelimited when info.TypeSymbol.SpecialType == SpecialType.System_String => $"{CountStringMethodRef}({memberName})",
                WireType.LengthDelimited when info.TypeSymbol is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte } => $"{CountBytesMethodRef}({memberName})",
                WireType.LengthDelimited when info.ExtraTypeInfo.Count is not 0 => $"{TypeInfoPropertyName}.Fields[{field << 3 | (byte)info.WireType}].Measure({memberName})",
                _ => $"{MeasureMethodRef}({field}, {WireTypeTypeRef}.{info.WireType}, {memberName})"
            };
        }
        
        private static string GenerateIfNotNullExpression(string variableName, string left, string right) => $"({variableName} != null ? {left} : {right})";

        private string GenerateIfNotDefaultExpression(ITypeSymbol typeSymbol, string variableName, string left, string right)
        {
            string fullTypeName = typeSymbol.GetFullName();
            return $"(!global::System.Collections.Generic.EqualityComparer<{fullTypeName}>.Default.Equals({variableName}, default) ? {left} : {right})";
        }

        private string GenerateShouldSerializeExpression(uint tag, string left, string right) =>
            $"({_fullQualifiedName}.{TypeInfoPropertyName}.Fields[{tag}].{ShouldSerializeTypeRef}({ObjectVarName}, {parser.IgnoreDefaultFields.ToString().ToLower()}) ? {left} : {right})";
    }
}