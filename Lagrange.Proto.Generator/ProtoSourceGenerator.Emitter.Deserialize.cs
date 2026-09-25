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
        private const string ProtoReaderTypeRef = "global::Lagrange.Proto.Primitives.ProtoReader";
        private const string ProtoSerializerTypeRef = "global::Lagrange.Proto.Serialization.ProtoSerializer";

        private const string ReaderVarName = "reader";

        private const string DecodeVarIntMethodName = "DecodeVarInt";
        private const string DecodeVarIntUnsafeMethodName = "DecodeVarIntUnsafe";
        private const string DecodeFixed32MethodName = "DecodeFixed32";
        private const string DecodeFixed64MethodName = "DecodeFixed64";
        private const string CreateSpanMethodName = "CreateSpan";
        private const string SkipFieldMethodName = "SkipField";

        private const string ZigZagDecodeMethodRef = $"{ProtoHelperTypeRef}.ZigZagDecode";

        private void EmitDeserializeMethod(SourceWriter source)
        {
            source.WriteLine($"public static void DeserializeHandler({_fullQualifiedName} {ObjectVarName}, ref {ProtoReaderTypeRef} {ReaderVarName})");
            source.WriteLine("{");
            source.Indentation++;

            source.WriteLine($"while (!{ReaderVarName}.IsCompleted)");
            source.WriteLine("{");
            source.Indentation++;

            source.WriteLine($"uint tag = {ReaderVarName}.{DecodeVarIntUnsafeMethodName}<uint>();");
            source.WriteLine("switch (tag)");
            source.WriteLine("{");
            source.Indentation++;

            foreach (var kv in parser.Fields)
            {
                int field = kv.Key;
                var info = kv.Value;

                EmitDeserializeCase(source, field, info);
            }

            
            source.WriteLine("default:");
            source.Indentation++;
            source.WriteLine($"{ReaderVarName}.{SkipFieldMethodName}(({WireTypeTypeRef})(tag & 0x07));");
            source.WriteLine("break;");
            source.Indentation--;

            source.Indentation--;
            source.WriteLine("}"); 

            source.Indentation--;
            source.WriteLine("}"); 

            source.Indentation--;
            source.WriteLine("}"); 
        }

        private void EmitDeserializeCase(SourceWriter source, int field, ProtoFieldInfo info)
        {
            uint tag = (uint)field << 3 | (byte)info.WireType;
            source.WriteLine($"case {tag}:");
            source.Indentation++;

            EmitDeserializeMember(source, field, info);

            source.WriteLine("break;");
            source.Indentation--;
        }

        private void EmitDeserializeMember(SourceWriter source, int field, ProtoFieldInfo info)
        {
            
            if (SymbolResolver.IsMapType(info.TypeSymbol, out _, out _) ||
                SymbolResolver.IsRepeatedType(info.TypeSymbol, out _) ||
                SymbolResolver.IsNodesType(info.TypeSymbol))
            {
                uint tag = (uint)field << 3 | (byte)info.WireType;
                source.WriteLine($"{TypeInfoPropertyName}.Fields[{tag}].Read(ref {ReaderVarName}, {ObjectVarName});");
                return;
            }

            string memberName = $"{ObjectVarName}.{info.Symbol.Name}";
            var typeSymbol = info.TypeSymbol;

            
            bool isNullable = typeSymbol.IsValueType && typeSymbol.IsNullable();
            if (isNullable && SymbolResolver.IsNullableType(typeSymbol, out var underlyingType))
            {
                typeSymbol = underlyingType;
            }

            
            switch (info.WireType)
            {
                case WireType.VarInt:
                    EmitVarIntRead(source, memberName, typeSymbol, info.IsSigned);
                    break;

                case WireType.Fixed32:
                    EmitFixed32Read(source, memberName, typeSymbol, info.IsSigned);
                    break;

                case WireType.Fixed64:
                    EmitFixed64Read(source, memberName, typeSymbol, info.IsSigned);
                    break;

                case WireType.LengthDelimited:
                    EmitLengthDelimitedRead(source, memberName, typeSymbol, info, field);
                    break;

                default:
                    
                    uint tag = (uint)field << 3 | (byte)info.WireType;
                    source.WriteLine($"{TypeInfoPropertyName}.Fields[{tag}].Read(ref {ReaderVarName}, {ObjectVarName});");
                    break;
            }
        }

        private void EmitVarIntRead(SourceWriter source, string memberName, ITypeSymbol typeSymbol, bool isSigned)
        {
            
            if (typeSymbol.SpecialType == SpecialType.System_Boolean)
            {
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine($"byte __b = {ReaderVarName}.{DecodeVarIntMethodName}<byte>();");
                source.WriteLine($"{memberName} = global::System.Runtime.CompilerServices.Unsafe.As<byte, bool>(ref __b);");
                source.Indentation--;
                source.WriteLine("}");
                return;
            }

            
            if (typeSymbol.TypeKind == TypeKind.Enum)
            {
                var underlyingType = ((INamedTypeSymbol)typeSymbol).EnumUnderlyingType;
                string enumTypeName = typeSymbol.GetFullName();
                string underlyingTypeName = GetDecodeTypeName(underlyingType!);
                source.WriteLine($"{memberName} = ({enumTypeName}){ReaderVarName}.{DecodeVarIntMethodName}<{underlyingTypeName}>();");
                return;
            }

            
            if (isSigned && typeSymbol.IsIntegerType())
            {
                string unsignedType = GetUnsignedTypeName(typeSymbol);
                string signedType = GetDecodeTypeName(typeSymbol);
                source.WriteLine($"{memberName} = {ZigZagDecodeMethodRef}(({signedType}){ReaderVarName}.{DecodeVarIntMethodName}<{unsignedType}>());");
                return;
            }

            
            string typeName = GetDecodeTypeName(typeSymbol);
            source.WriteLine($"{memberName} = {ReaderVarName}.{DecodeVarIntMethodName}<{typeName}>();");
        }

        private void EmitFixed32Read(SourceWriter source, string memberName, ITypeSymbol typeSymbol, bool isSigned)
        {
            if (isSigned && typeSymbol.IsIntegerType())
            {
                string unsignedType = GetUnsignedTypeName(typeSymbol);
                string signedType = GetDecodeTypeName(typeSymbol);
                source.WriteLine($"{memberName} = {ZigZagDecodeMethodRef}(({signedType}){ReaderVarName}.{DecodeFixed32MethodName}<{unsignedType}>());");
                return;
            }

            string typeName = GetDecodeTypeName(typeSymbol);
            source.WriteLine($"{memberName} = {ReaderVarName}.{DecodeFixed32MethodName}<{typeName}>();");
        }

        private void EmitFixed64Read(SourceWriter source, string memberName, ITypeSymbol typeSymbol, bool isSigned)
        {
            if (isSigned && typeSymbol.IsIntegerType())
            {
                string unsignedType = GetUnsignedTypeName(typeSymbol);
                string signedType = GetDecodeTypeName(typeSymbol);
                source.WriteLine($"{memberName} = {ZigZagDecodeMethodRef}(({signedType}){ReaderVarName}.{DecodeFixed64MethodName}<{unsignedType}>());");
                return;
            }

            string typeName = GetDecodeTypeName(typeSymbol);
            source.WriteLine($"{memberName} = {ReaderVarName}.{DecodeFixed64MethodName}<{typeName}>();");
        }

        private void EmitLengthDelimitedRead(SourceWriter source, string memberName, ITypeSymbol typeSymbol, ProtoFieldInfo info, int field)
        {
            
            if (typeSymbol.SpecialType == SpecialType.System_String)
            {
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine($"int __len = {ReaderVarName}.{DecodeVarIntMethodName}<int>();");
                source.WriteLine($"var __span = {ReaderVarName}.{CreateSpanMethodName}(__len);");
                source.WriteLine($"{memberName} = __span.IsEmpty ? string.Empty : global::System.Text.Encoding.UTF8.GetString(__span);");
                source.Indentation--;
                source.WriteLine("}");
                return;
            }

            
            if (typeSymbol is IArrayTypeSymbol { ElementType.SpecialType: SpecialType.System_Byte })
            {
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine($"int __len = {ReaderVarName}.{DecodeVarIntMethodName}<int>();");
                source.WriteLine("if (__len == 0)");
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine($"{memberName} = global::System.Array.Empty<byte>();");
                source.Indentation--;
                source.WriteLine("}");
                source.WriteLine("else");
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine("var __buffer = global::System.GC.AllocateUninitializedArray<byte>(__len);");
                source.WriteLine($"var __span = {ReaderVarName}.{CreateSpanMethodName}(__len);");
                source.WriteLine("__span.CopyTo(__buffer);");
                source.WriteLine($"{memberName} = __buffer;");
                source.Indentation--;
                source.WriteLine("}");
                source.Indentation--;
                source.WriteLine("}");
                return;
            }

            
            if (typeSymbol is INamedTypeSymbol { Name: "Memory", IsGenericType: true } memoryType &&
                memoryType.TypeArguments[0].SpecialType == SpecialType.System_Byte)
            {
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine($"int __len = {ReaderVarName}.{DecodeVarIntMethodName}<int>();");
                source.WriteLine("if (__len == 0)");
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine($"{memberName} = global::System.Memory<byte>.Empty;");
                source.Indentation--;
                source.WriteLine("}");
                source.WriteLine("else");
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine("var __buffer = global::System.GC.AllocateUninitializedArray<byte>(__len);");
                source.WriteLine($"var __span = {ReaderVarName}.{CreateSpanMethodName}(__len);");
                source.WriteLine("__span.CopyTo(__buffer);");
                source.WriteLine($"{memberName} = __buffer;");
                source.Indentation--;
                source.WriteLine("}");
                source.Indentation--;
                source.WriteLine("}");
                return;
            }

            
            if (typeSymbol is INamedTypeSymbol { Name: "ReadOnlyMemory", IsGenericType: true } readOnlyMemoryType &&
                readOnlyMemoryType.TypeArguments[0].SpecialType == SpecialType.System_Byte)
            {
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine($"int __len = {ReaderVarName}.{DecodeVarIntMethodName}<int>();");
                source.WriteLine("if (__len == 0)");
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine($"{memberName} = global::System.ReadOnlyMemory<byte>.Empty;");
                source.Indentation--;
                source.WriteLine("}");
                source.WriteLine("else");
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine("var __buffer = global::System.GC.AllocateUninitializedArray<byte>(__len);");
                source.WriteLine($"var __span = {ReaderVarName}.{CreateSpanMethodName}(__len);");
                source.WriteLine("__span.CopyTo(__buffer);");
                source.WriteLine($"{memberName} = __buffer;");
                source.Indentation--;
                source.WriteLine("}");
                source.Indentation--;
                source.WriteLine("}");
                return;
            }

            
            if (SymbolResolver.IsProtoPackable(typeSymbol))
            {
                string typeName = typeSymbol.GetFullName();
                source.WriteLine("{");
                source.Indentation++;
                source.WriteLine($"int __len = {ReaderVarName}.{DecodeVarIntMethodName}<int>();");
                source.WriteLine($"var __span = {ReaderVarName}.{CreateSpanMethodName}(__len);");
                source.WriteLine($"{memberName} = {ProtoSerializerTypeRef}.DeserializeProtoPackable<{typeName}>(__span);");
                source.Indentation--;
                source.WriteLine("}");
                return;
            }

            
            uint tag = (uint)field << 3 | (byte)info.WireType;
            source.WriteLine($"{TypeInfoPropertyName}.Fields[{tag}].Read(ref {ReaderVarName}, {ObjectVarName});");
        }

        private static string GetDecodeTypeName(ITypeSymbol typeSymbol)
        {
            return typeSymbol.SpecialType switch
            {
                SpecialType.System_Byte => "byte",
                SpecialType.System_SByte => "sbyte",
                SpecialType.System_Int16 => "short",
                SpecialType.System_UInt16 => "ushort",
                SpecialType.System_Int32 => "int",
                SpecialType.System_UInt32 => "uint",
                SpecialType.System_Int64 => "long",
                SpecialType.System_UInt64 => "ulong",
                SpecialType.System_Single => "float",
                SpecialType.System_Double => "double",
                _ => typeSymbol.GetFullName()
            };
        }

        private static string GetUnsignedTypeName(ITypeSymbol typeSymbol)
        {
            return typeSymbol.SpecialType switch
            {
                SpecialType.System_SByte => "byte",
                SpecialType.System_Int16 => "ushort",
                SpecialType.System_Int32 => "uint",
                SpecialType.System_Int64 => "ulong",
                SpecialType.System_Byte => "byte",
                SpecialType.System_UInt16 => "ushort",
                SpecialType.System_UInt32 => "uint",
                SpecialType.System_UInt64 => "ulong",
                _ => "uint"
            };
        }
    }
}
