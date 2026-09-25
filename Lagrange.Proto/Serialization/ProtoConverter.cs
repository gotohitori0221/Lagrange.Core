using Lagrange.Proto.Primitives;

namespace Lagrange.Proto.Serialization;

public abstract class ProtoConverter
{
}

public abstract class ProtoConverter<T> : ProtoConverter
{
    public virtual bool ShouldSerialize(T value, bool ignoreDefaultValue) => value != null;
    
    public abstract void Write(int field, WireType wireType, ProtoWriter writer, T value);
    
    public virtual void WriteWithNumberHandling(int field, WireType wireType, ProtoWriter writer, T value, ProtoNumberHandling numberHandling) => Write(field, wireType, writer, value);

    public abstract int Measure(int field, WireType wireType, T value);
    
    public virtual int MeasureWithNumberHandling(int field, WireType wireType, T value, ProtoNumberHandling numberHandling) => Measure(field, wireType, value);
    
    public abstract T Read(int field, WireType wireType, ref ProtoReader reader);
    
    public virtual T ReadWithNumberHandling(int field, WireType wireType, ref ProtoReader reader, ProtoNumberHandling numberHandling) => Read(field, wireType, ref reader);
}