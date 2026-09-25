using Lagrange.Proto.Serialization;

namespace Lagrange.Proto;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ProtoMemberAttribute(int field) : Attribute
{
    public int Field { get; } = field;
    
    public ProtoNumberHandling NumberHandling { get; init; } = ProtoNumberHandling.Default;
    
    public WireType NodesWireType { get; init; }
}