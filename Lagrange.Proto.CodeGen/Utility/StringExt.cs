namespace Lagrange.Proto.CodeGen.Utility;

public static class StringExt
{
    public static string ToPascalCase(this string str)
    {
        if (string.IsNullOrEmpty(str)) return str;
        if (str.Length == 1) return str.ToUpper();
        return char.ToUpper(str[0]) + str[1..];
    }

    public static string NormailizePackageToNamespace(this string package)
    {
        if (string.IsNullOrEmpty(package)) return package;
        var parts = package.Split('.');
        for (int i = 0; i < parts.Length; i++)
        {
            parts[i] = parts[i].ToPascalCase();
        }

        return string.Join(".", parts);
    }
}