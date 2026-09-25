using System.Buffers;
using Lagrange.Proto.Primitives;

namespace Lagrange.Proto.Nodes;

public partial class ProtoNode
{
    public abstract void WriteTo(int field, ProtoWriter writer);

    public abstract int Measure(int field);
}