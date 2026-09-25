using Lagrange.Proto.Serialization;

namespace Lagrange.Proto;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ProtoValueMemberAttribute : Attribute
{
    public ProtoNumberHandling NumberHandling { get; init; } = ProtoNumberHandling.Default;
    
    public WireType? NodesWireType { get; init; }
}