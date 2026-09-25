using Lagrange.Proto.Nodes;

namespace Lagrange.Proto.Runner;

internal static partial class Program
{
    private static void Main(string[] args)
    {
        
        VarIntPerformanceTest.Run();

        Console.WriteLine("\n=== Original ProtoObject Test ===");
        var test = new ProtoObject()
        {
            { 1, new ProtoObject{ { 1, 2 } } },
            { 1, new ProtoObject{ { 1, 2 } } },
            { 3, 4 },
            { 5, 6 }
        };

        var bytes = test.Serialize();
        Console.WriteLine(Convert.ToHexString(bytes));
        var parsed = ProtoObject.Parse(bytes);

        int value = parsed[1][0][1].GetValue<int>();
        Console.WriteLine($"Parsed value: {value}");
    }
}