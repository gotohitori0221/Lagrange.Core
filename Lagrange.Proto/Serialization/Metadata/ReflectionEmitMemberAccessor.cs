using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace Lagrange.Proto.Serialization.Metadata;

[RequiresDynamicCode(ProtoSerializer.SerializationRequiresDynamicCodeMessage)]
[RequiresUnreferencedCode(ProtoSerializer.SerializationRequiresDynamicCodeMessage)]
internal sealed class ReflectionEmitMemberAccessor : MemberAccessor
{
    private static readonly Type ObjectType = typeof(object);
    
    public override Func<T>? CreateParameterlessConstructor<T>(ConstructorInfo? constructorInfo)
    {
        Debug.Assert(constructorInfo is null || constructorInfo.GetParameters().Length == 0);

        var type = typeof(T);
        if (type.IsAbstract) return null;
        if (constructorInfo is null && !type.IsValueType) return null;

        var dynamicMethod = new DynamicMethod(ConstructorInfo.ConstructorName, type, Type.EmptyTypes, typeof(ReflectionEmitMemberAccessor).Module, skipVisibility: true);
        var il = dynamicMethod.GetILGenerator();
        if (constructorInfo is null)
        {
            Debug.Assert(type.IsValueType);

            var local = il.DeclareLocal(type);

            il.Emit(OpCodes.Ldloca_S, local);
            il.Emit(OpCodes.Initobj, type);
            il.Emit(OpCodes.Ldloc, local);
        }
        else
        {
            il.Emit(OpCodes.Newobj, constructorInfo);
        }
        
        il.Emit(OpCodes.Ret);

        return CreateDelegate<Func<T>>(dynamicMethod);
    }
    
    public override Func<object, TProperty> CreatePropertyGetter<TProperty>(PropertyInfo propertyInfo) =>
        CreateDelegate<Func<object, TProperty>>(CreatePropertyGetter(propertyInfo, typeof(TProperty)));

    private static DynamicMethod CreatePropertyGetter(PropertyInfo propertyInfo, Type runtimePropertyType)
    {
        var realMethod = propertyInfo.GetMethod;
        var declaringType = propertyInfo.DeclaringType;
        var declaredPropertyType = propertyInfo.PropertyType;
        Debug.Assert(realMethod != null);
        Debug.Assert(declaringType != null);

        var dynamicMethod = CreateGetterMethod(propertyInfo.Name, runtimePropertyType);
        var il = dynamicMethod.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);

        if (declaringType.IsValueType)
        {
            il.Emit(OpCodes.Unbox, declaringType);
            il.Emit(OpCodes.Call, realMethod);
        }
        else
        {
            il.Emit(OpCodes.Castclass, declaringType);
            il.Emit(OpCodes.Callvirt, realMethod);
        }
        
        if (declaredPropertyType != runtimePropertyType && declaredPropertyType.IsValueType)
        {
            Debug.Assert(!runtimePropertyType.IsValueType);

            il.Emit(OpCodes.Box, declaredPropertyType);
        }

        il.Emit(OpCodes.Ret);

        return dynamicMethod;
    }

    public override Action<object, TProperty> CreatePropertySetter<TProperty>(PropertyInfo propertyInfo) =>
        CreateDelegate<Action<object, TProperty>>(CreatePropertySetter(propertyInfo, typeof(TProperty)));

    private static DynamicMethod CreatePropertySetter(PropertyInfo propertyInfo, Type runtimePropertyType)
    {
        var realMethod = propertyInfo.SetMethod;
        var declaringType = propertyInfo.DeclaringType;
        var declaredPropertyType = propertyInfo.PropertyType;
        Debug.Assert(realMethod != null);
        Debug.Assert(declaringType != null);

        var dynamicMethod = CreateSetterMethod(propertyInfo.Name, runtimePropertyType);
        var il = dynamicMethod.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(declaringType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, declaringType);
        il.Emit(OpCodes.Ldarg_1);

        if (declaredPropertyType != runtimePropertyType && declaredPropertyType.IsValueType)
        {
            Debug.Assert(!runtimePropertyType.IsValueType);
            il.Emit(OpCodes.Unbox_Any, declaredPropertyType);
        }

        il.Emit(declaringType.IsValueType ? OpCodes.Call : OpCodes.Callvirt, realMethod);
        il.Emit(OpCodes.Ret);

        return dynamicMethod;
    }

    public override Func<object, TProperty> CreateFieldGetter<TProperty>(FieldInfo fieldInfo) =>
        CreateDelegate<Func<object, TProperty>>(CreateFieldGetter(fieldInfo, typeof(TProperty)));

    private static DynamicMethod CreateFieldGetter(FieldInfo fieldInfo, Type runtimeFieldType)
    {
        var declaringType = fieldInfo.DeclaringType;
        var declaredFieldType = fieldInfo.FieldType;
        Debug.Assert(declaringType != null);

        var dynamicMethod = CreateGetterMethod(fieldInfo.Name, runtimeFieldType);
        var il = dynamicMethod.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(declaringType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, declaringType);
        il.Emit(OpCodes.Ldfld, fieldInfo);

        if (declaredFieldType.IsValueType && declaredFieldType != runtimeFieldType)
        {
            il.Emit(OpCodes.Box, declaredFieldType);
        }

        il.Emit(OpCodes.Ret);

        return dynamicMethod;
    }

    public override Action<object, TProperty> CreateFieldSetter<TProperty>(FieldInfo fieldInfo) =>
        CreateDelegate<Action<object, TProperty>>(CreateFieldSetter(fieldInfo, typeof(TProperty)));

    private static DynamicMethod CreateFieldSetter(FieldInfo fieldInfo, Type runtimeFieldType)
    {
        var declaringType = fieldInfo.DeclaringType;
        var declaredFieldType = fieldInfo.FieldType;
        Debug.Assert(declaringType != null);

        var dynamicMethod = CreateSetterMethod(fieldInfo.Name, runtimeFieldType);
        var il = dynamicMethod.GetILGenerator();

        il.Emit(OpCodes.Ldarg_0);
        il.Emit(declaringType.IsValueType ? OpCodes.Unbox : OpCodes.Castclass, declaringType);
        il.Emit(OpCodes.Ldarg_1);
        

        if (declaredFieldType != runtimeFieldType && declaredFieldType.IsValueType)
        {
            il.Emit(OpCodes.Unbox_Any, declaredFieldType);
        }

        il.Emit(OpCodes.Stfld, fieldInfo);
        il.Emit(OpCodes.Ret);

        return dynamicMethod;
    }

    private static DynamicMethod CreateGetterMethod(string memberName, Type memberType) =>
        new($"{memberName}Getter", memberType, [ObjectType], typeof(ReflectionEmitMemberAccessor).Module, skipVisibility: true);

    private static DynamicMethod CreateSetterMethod(string memberName, Type memberType) =>
        new($"{memberName}Setter", typeof(void), [ObjectType, memberType], typeof(ReflectionEmitMemberAccessor).Module, skipVisibility: true);

    [return: NotNullIfNotNull(nameof(method))]
    private static T? CreateDelegate<T>(DynamicMethod? method) where T : Delegate => (T?)method?.CreateDelegate(typeof(T));
}
