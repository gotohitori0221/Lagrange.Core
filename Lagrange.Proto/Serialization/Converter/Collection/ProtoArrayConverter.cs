using System.Diagnostics;

namespace Lagrange.Proto.Serialization.Converter;

public class ProtoArrayConverter<T> : ProtoRepeatedConverter<T[], T>
{
    private protected override T[] Create() => [];
    
    private protected override object CreateState() => new List<T>();

    private protected override void Add(T item, T[] collection, object? state)
    {
        Debug.Assert(state is List<T>);
        ((List<T>)state).Add(item);
    }

    private protected override T[] Finalize(T[] collection, object? state)
    {
        Debug.Assert(state is List<T>);
        return ((List<T>)state).ToArray();
    }
}