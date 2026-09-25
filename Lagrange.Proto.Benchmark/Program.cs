using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Lagrange.Proto.Serialization;
using ProtoBuf;
using ProtoAttribute = ProtoBuf.ProtoMemberAttribute;

namespace Lagrange.Proto.Benchmark;

public static class Program
{
    private static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<ProtoBenchmark>();
        Console.WriteLine(summary);
    }
}

[MemoryDiagnoser]
public class ProtoBenchmark
{
    private Test _commonObject = new()
    {
        Test1 = 114514,
        Test2 = "Test",
        Test3 = 3.14f,
        Test4 = 3.14,
        Test5 = 5,
        Test6 = "Test6",
        Test7 = new byte[] { 1, 2, 3 },
        Test8 = new Test_2
        {
            Test1 = 1,
            Test2 = "Test",
            Test3 = 3.14f,
            Test4 = 3.14,
            Test5 = 5,
            Test6 = "Test6",
            Test7 = new byte[] { 1, 2, 3 }
        },
        Test13 = true
    };
    
    [Benchmark]
    public void ProtoBufTest()
    {
        var arrayBufferWriter = new ArrayBufferWriter<byte>();
        Serializer.Serialize(arrayBufferWriter, _commonObject);
        var test2 = Serializer.Deserialize<Test>(arrayBufferWriter.WrittenMemory.Span);
    }
    
    [Benchmark]
    public void ProtoPackableTest()
    {
        var bytes = ProtoSerializer.SerializeProtoPackable(_commonObject);
        var test2 = ProtoSerializer.DeserializeProtoPackable<Test>(bytes);
    }
    
    [Benchmark]
    public void ProtoTestReflection()
    {
        var bytes = ProtoSerializer.Serialize(_commonObject);
        var test2 = ProtoSerializer.Deserialize<Test>(bytes);
    }
    
    [Benchmark]
    public void ProtoBufTestMultipleTimes()
    {
        for (int i = 0; i < 10000; i++)
        {
            var arrayBufferWriter = new ArrayBufferWriter<byte>();
            Serializer.Serialize(arrayBufferWriter, _commonObject);
            var test2 = Serializer.Deserialize<Test>(arrayBufferWriter.WrittenMemory.Span);
        }
    }
    
    [Benchmark]
    public void ProtoPackableTestMultipleTimes()
    {
        for (int i = 0; i < 10000; i++)
        {
            var bytes = ProtoSerializer.SerializeProtoPackable(_commonObject);
            var test2 = ProtoSerializer.DeserializeProtoPackable<Test>(bytes);
        }
    }
    
    [Benchmark]
    public void ProtoTestReflectionMultipleTimes()
    {
        for (int i = 0; i < 10000; i++)
        {
            var bytes = ProtoSerializer.Serialize(_commonObject);
            var test2 = ProtoSerializer.Deserialize<Test>(bytes);
        }
    }
}

[ProtoPackable]
[ProtoContract]
public partial class Test
{
    [ProtoMember(1, NumberHandling = ProtoNumberHandling.Signed)]  [Proto(1, DataFormat = DataFormat.ZigZag)]
    public int Test1 { get; set; }
        
    [ProtoMember(2)] [Proto(2)] public string Test2 { get; set; } = string.Empty;
    
    [ProtoMember(3)] [Proto(3)] public float Test3 { get; set; }

    [ProtoMember(4)] [Proto(4)] public double Test4 { get; set; }
    
    [ProtoMember(5)] [Proto(5)] public int? Test5 { get; set; }
    
    [ProtoMember(6)] [Proto(6)] public string? Test6 { get; set; }
    
    [ProtoMember(7)] [Proto(7)] public byte[] Test7 { get; set; } = [];
    
    [ProtoMember(8)] [Proto(8)] public Test_2 Test8 { get; set; } = new();
    
    [ProtoMember(13)] [Proto(13)] public bool Test13 { get; set; } = true;
    
    [ProtoMember(14)] [Proto(14)] public long Test14 { get; set; }
}


[ProtoPackable(IgnoreDefaultFields = true)]
[ProtoContract]
public partial class Test_2
{
    [ProtoMember(1, NumberHandling = ProtoNumberHandling.Signed)]  [Proto(1, DataFormat = DataFormat.ZigZag)]
    public int Test1 { get; set; }
        
    [ProtoMember(2)] [Proto(2)] public string Test2 { get; set; } = string.Empty;
    
    [ProtoMember(3)] [Proto(3)] public float Test3 { get; set; }

    [ProtoMember(4)] [Proto(4)] public double Test4 { get; set; }
    
    [ProtoMember(5)] [Proto(5)] public int? Test5 { get; set; }
    
    [ProtoMember(6)] [Proto(6)] public string? Test6 { get; set; }
    
    [ProtoMember(7)] [Proto(7)] public byte[] Test7 { get; set; } = [];
}