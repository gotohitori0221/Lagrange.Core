using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace Lagrange.Proto.Serialization.Metadata;

[RequiresDynamicCode(ProtoSerializer.SerializationRequiresDynamicCodeMessage)]
[RequiresUnreferencedCode(ProtoSerializer.SerializationRequiresDynamicCodeMessage)]
[ExcludeFromCodeCoverage] 
internal sealed class ReflectionMemberAccessor : MemberAccessor
{
    public override Func<T>? CreateParameterlessConstructor<T>(ConstructorInfo? constructorInfo)
    {
        Debug.Assert(typeof(T) != null);
        Debug.Assert(constructorInfo is null || constructorInfo.GetParameters().Length == 0);

        if (typeof(T).IsAbstract) return null;

        return constructorInfo is null
            ? typeof(T).IsValueType ? Activator.CreateInstance<T> : null
            : () => (T)constructorInfo.Invoke(null);
    }

    public override Func<object, TProperty> CreatePropertyGetter<TProperty>(PropertyInfo propertyInfo)
    {
        var getMethodInfo = propertyInfo.GetMethod!;

        return obj => (TProperty)getMethodInfo.Invoke(obj, null)!;
    }

    public override Action<object, TProperty> CreatePropertySetter<TProperty>(PropertyInfo propertyInfo)
    {
        var setMethodInfo = propertyInfo.SetMethod!;

        return delegate (object obj, TProperty value) { setMethodInfo.Invoke(obj, [value!]); };
    }

    public override Func<object, TProperty> CreateFieldGetter<TProperty>(FieldInfo fieldInfo) =>
        obj => (TProperty)fieldInfo.GetValue(obj)!;

    public override Action<object, TProperty> CreateFieldSetter<TProperty>(FieldInfo fieldInfo) =>
        delegate (object obj, TProperty value) { fieldInfo.SetValue(obj, value); };
}