using System.Reflection;

namespace Lagrange.Proto.Serialization.Metadata;

internal abstract class MemberAccessor
{
    public abstract Func<T>? CreateParameterlessConstructor<T>(ConstructorInfo? constructorInfo);

    public abstract Func<object, TProperty> CreatePropertyGetter<TProperty>(PropertyInfo propertyInfo);

    public abstract Action<object, TProperty> CreatePropertySetter<TProperty>(PropertyInfo propertyInfo);

    public abstract Func<object, TProperty> CreateFieldGetter<TProperty>(FieldInfo fieldInfo);

    public abstract Action<object, TProperty> CreateFieldSetter<TProperty>(FieldInfo fieldInfo);

    public virtual void Clear() { }
}