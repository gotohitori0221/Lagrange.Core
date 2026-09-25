using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Lagrange.Proto.Serialization.Metadata;

[method: RequiresDynamicCode(ProtoSerializer.SerializationRequiresDynamicCodeMessage)]
[method: RequiresUnreferencedCode(ProtoSerializer.SerializationRequiresDynamicCodeMessage)]
internal sealed partial class ReflectionEmitCachingMemberAccessor() : MemberAccessor
{
    private readonly ReflectionEmitMemberAccessor _sourceAccessor = new();
    private readonly Cache<(string id, Type declaringType, MemberInfo? member)> _cache = new(slidingExpiration: TimeSpan.FromMilliseconds(1000), evictionInterval: TimeSpan.FromMilliseconds(200));

    public override void Clear() => _cache.Clear();
    
    public override Func<T>? CreateParameterlessConstructor<T>(ConstructorInfo? constructorInfo) =>
        _cache.GetOrAdd(
            key: (nameof(CreateParameterlessConstructor), typeof(T), constructorInfo),
            valueFactory: key => _sourceAccessor.CreateParameterlessConstructor<T>((ConstructorInfo)key.member!));

    public override Func<object, TProperty> CreateFieldGetter<TProperty>(FieldInfo fieldInfo) =>
        _cache.GetOrAdd(
            key: (nameof(CreateFieldGetter), typeof(TProperty), fieldInfo),
            valueFactory: key => _sourceAccessor.CreateFieldGetter<TProperty>((FieldInfo)key.member!));

    public override Action<object, TProperty> CreateFieldSetter<TProperty>(FieldInfo fieldInfo) =>
        _cache.GetOrAdd(
            key: (nameof(CreateFieldSetter), typeof(TProperty), fieldInfo),
            valueFactory: key => _sourceAccessor.CreateFieldSetter<TProperty>((FieldInfo)key.member!));
    
    public override Func<object, TProperty> CreatePropertyGetter<TProperty>(PropertyInfo propertyInfo) =>
        _cache.GetOrAdd(
            key: (nameof(CreatePropertyGetter), typeof(TProperty), propertyInfo),
            valueFactory: key => _sourceAccessor.CreatePropertyGetter<TProperty>((PropertyInfo)key.member!));

    public override Action<object, TProperty> CreatePropertySetter<TProperty>(PropertyInfo propertyInfo) =>
        _cache.GetOrAdd(
            key: (nameof(CreatePropertySetter), typeof(TProperty), propertyInfo),
            valueFactory: key => _sourceAccessor.CreatePropertySetter<TProperty>((PropertyInfo)key.member!));
}
