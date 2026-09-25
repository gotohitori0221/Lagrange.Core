using System.Reflection;

namespace Lagrange.Milky;

public static class Constants
{
    public static readonly string GitHash = GetGitHash(9);
    public static readonly string MilkyVersion = "1.3";

    public const long RetcodeInvalidParameter = -400;

    public const long RetcodeNotLoggedIn = -403;

    public const long RetcodeOperationFailed = -404;

    private static string GetGitHash(int length)
    {
        var assembly = Assembly.GetAssembly(typeof(Program));
        if (assembly == null) return "unknown";

        var attribute = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        if (attribute == null) return "unknown";

        string version = attribute.InformationalVersion;

        int plusIndex = version.LastIndexOf('+');
        if (plusIndex < 0) return "unknown";

        return version.Substring(plusIndex + 1, length);
    }
}