namespace Lagrange.Core.Utility.Extension;

internal static class ReflectionExt
{
    public static bool HasImplemented<TInterface>(this Type type)
    {
        if (type is { IsGenericTypeDefinition: false, IsAbstract: false, IsInterface: false })
        {
            return type.IsAssignableTo(typeof(TInterface));
        }

        return false;
    }
}