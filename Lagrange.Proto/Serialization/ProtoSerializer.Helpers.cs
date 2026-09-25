namespace Lagrange.Proto.Serialization;

public static partial class ProtoSerializer
{
    internal const string SerializationUnreferencedCodeMessage = "Proto serialization and deserialization might require types that cannot be statically analyzed. Use the SerializePackable<T> that takes a IProtoSerializable<T> to ensure generated code is used, or make sure all of the required types are preserved.";
    internal const string SerializationRequiresDynamicCodeMessage = "Proto serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use Lagrange.Proto source generation for native AOT applications.";
}