namespace Lagrange.Proto.Serialization.Converter;

public class ProtoListConverter<T> : ProtoRepeatedConverter<List<T>, T>
{
    private protected override List<T> Create() => [];

    private protected override void Add(T item, List<T> collection, object? state) => collection.Add(item);
}