using System.Buffers;
using Lagrange.Proto.Serialization;

namespace Lagrange.Proto.Test;

[TestFixture]
public class ProtoTest
{
    private Test _test;
    
    private byte[] _bytes;

    private byte[] _srcGenBytes;
    
    [SetUp]
    public void Setup()
    {
        var test = new Test
        {
            Test1 = 114514,
            Test2 = "Test",
            Test3 = 3.14f,
            Test4 = 3.14,
            Test5 = 5,
            Test6 = "Test6",
            Test7 = [1, 2, 3],
            Test8 = new Test_2
            {
                Test1 = 1,
                Test2 = "Test",
                Test3 = 3.14f,
                Test4 = 3.14,
                Test5 = 5,
                Test6 = "Test6",
                Test7 = [1, 2, 3],
                Test8 = TestEnum.Test8
            },
            Test9 = new byte[] { 1, 2, 3 },
            Test10 = new byte[] { 1, 2, 3 },
            Test11 = new char[] { '1', '2', '3' },
            Test12 = new char[] { '1', '2', '3' },
            Test13 = true,
            Test14 = 1234567890123456789
        };
        
        _test = test;
        _bytes = ProtoSerializer.Serialize(test);
        _srcGenBytes = ProtoSerializer.SerializeProtoPackable(test);
    }
    
    [Test]
    public void TestEquality()
    {
        Console.WriteLine(Convert.ToHexString(_bytes));
        Console.WriteLine(Convert.ToHexString(_srcGenBytes));
        
        Assert.That(_bytes, Is.EqualTo(_srcGenBytes));
    }

    [Test]
    public void TestReflection()
    {
        var obj = ProtoSerializer.Deserialize<Test>(_bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(obj.Test1, Is.EqualTo(_test.Test1));
            Assert.That(obj.Test2, Is.EqualTo(_test.Test2));
            Assert.That(obj.Test3, Is.EqualTo(_test.Test3));
            Assert.That(obj.Test4, Is.EqualTo(_test.Test4));
            Assert.That(obj.Test5, Is.EqualTo(_test.Test5));
            Assert.That(obj.Test6, Is.EqualTo(_test.Test6));
            Assert.That(obj.Test7, Is.EqualTo(_test.Test7));
            Assert.That(obj.Test8.Test1, Is.EqualTo(_test.Test8.Test1));
            Assert.That(obj.Test8.Test2, Is.EqualTo(_test.Test8.Test2));
            Assert.That(obj.Test8.Test3, Is.EqualTo(_test.Test8.Test3));
            Assert.That(obj.Test8.Test4, Is.EqualTo(_test.Test8.Test4));
            Assert.That(obj.Test8.Test5, Is.EqualTo(_test.Test8.Test5));
            Assert.That(obj.Test8.Test6, Is.EqualTo(_test.Test8.Test6));
            Assert.That(obj.Test8.Test7, Is.EqualTo(_test.Test8.Test7));
            Assert.That(obj.Test8.Test8, Is.EqualTo(_test.Test8.Test8));
            Assert.That(obj.Test9.ToArray(), Is.EqualTo(_test.Test9.ToArray()));
            Assert.That(obj.Test10.ToArray(), Is.EqualTo(_test.Test10.ToArray()));
            Assert.That(obj.Test11.ToArray(), Is.EqualTo(_test.Test11.ToArray()));
            Assert.That(obj.Test12.ToArray(), Is.EqualTo(_test.Test12.ToArray()));
            Assert.That(obj.Test13, Is.EqualTo(_test.Test13));
            Assert.That(obj.Test14, Is.EqualTo(_test.Test14));
        });
    }
    
    [Test]
    public void TestSrcGen()
    {
        var obj = ProtoSerializer.DeserializeProtoPackable<Test>(_bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(obj.Test1, Is.EqualTo(_test.Test1));
            Assert.That(obj.Test2, Is.EqualTo(_test.Test2));
            Assert.That(obj.Test3, Is.EqualTo(_test.Test3));
            Assert.That(obj.Test4, Is.EqualTo(_test.Test4));
            Assert.That(obj.Test5, Is.EqualTo(_test.Test5));
            Assert.That(obj.Test6, Is.EqualTo(_test.Test6));
            Assert.That(obj.Test7, Is.EqualTo(_test.Test7));
            Assert.That(obj.Test8.Test1, Is.EqualTo(_test.Test8.Test1));
            Assert.That(obj.Test8.Test2, Is.EqualTo(_test.Test8.Test2));
            Assert.That(obj.Test8.Test3, Is.EqualTo(_test.Test8.Test3));
            Assert.That(obj.Test8.Test4, Is.EqualTo(_test.Test8.Test4));
            Assert.That(obj.Test8.Test5, Is.EqualTo(_test.Test8.Test5));
            Assert.That(obj.Test8.Test6, Is.EqualTo(_test.Test8.Test6));
            Assert.That(obj.Test8.Test7, Is.EqualTo(_test.Test8.Test7));
            Assert.That(obj.Test8.Test8, Is.EqualTo(_test.Test8.Test8));
            Assert.That(obj.Test9.ToArray(), Is.EqualTo(_test.Test9.ToArray()));
            Assert.That(obj.Test10.ToArray(), Is.EqualTo(_test.Test10.ToArray()));
            Assert.That(obj.Test11.ToArray(), Is.EqualTo(_test.Test11.ToArray()));
            Assert.That(obj.Test12.ToArray(), Is.EqualTo(_test.Test12.ToArray()));
            Assert.That(obj.Test13, Is.EqualTo(_test.Test13));
            Assert.That(obj.Test14, Is.EqualTo(_test.Test14));
        });
    }

    [Test]
    public void TestBufferedWriting()
    {
        var buffer = new ArrayBufferWriter<byte>();

        ProtoSerializer.Serialize(buffer, _test);
        var reflectionBytes = buffer.WrittenMemory.ToArray();
        buffer.Clear();
        
        ProtoSerializer.SerializeProtoPackable(buffer, _test);
        var srcGenBytes = buffer.WrittenMemory.ToArray();
        buffer.Clear();
        
        Assert.That(reflectionBytes, Is.EqualTo(srcGenBytes));
        Assert.That(reflectionBytes, Is.EqualTo(_bytes));
    }
}

[ProtoPackable]
public partial class Test
{
    [ProtoMember(1, NumberHandling = ProtoNumberHandling.Signed)] 
    public int Test1 { get; set; }
        
    [ProtoMember(2)] public string Test2 { get; set; } = string.Empty;
    
    [ProtoMember(3)] public float Test3 { get; set; }

    [ProtoMember(4)] public double Test4;
    
    [ProtoMember(5)] public int? Test5 { get; set; }
    
    [ProtoMember(6)] public string? Test6 { get; set; }
    
    [ProtoMember(7)] public byte[] Test7 { get; set; } = [];
    
    [ProtoMember(8)] public Test_2 Test8 { get; set; } = new();
    
    [ProtoMember(9)] public ReadOnlyMemory<byte> Test9 { get; set; } = Array.Empty<byte>();
    
    [ProtoMember(10)] public Memory<byte> Test10 { get; set; } = Array.Empty<byte>();
    
    [ProtoMember(11)] public ReadOnlyMemory<char> Test11 { get; set; } = Array.Empty<char>();
    
    [ProtoMember(12)] public Memory<char> Test12 { get; set; } = Array.Empty<char>();
    
    [ProtoMember(13)] public bool Test13 { get; set; } = true;
    
    [ProtoMember(14)] public long Test14 { get; set; }
}


[ProtoPackable(IgnoreDefaultFields = true)]
public partial class Test_2
{
    [ProtoMember(1, NumberHandling = ProtoNumberHandling.Signed)] 
    public int Test1 { get; set; }
        
    [ProtoMember(2)] public string Test2 { get; set; } = string.Empty;
    
    [ProtoMember(3)] public float Test3 { get; set; }
    
    [ProtoMember(4)] public double Test4 { get; set; }
    
    [ProtoMember(5)] public int? Test5 { get; set; }
    
    [ProtoMember(6)] public string? Test6 { get; set; }
    
    [ProtoMember(7)] public byte[] Test7 { get; set; } = [];
    
    [ProtoMember(8)] public TestEnum Test8 { get; set; } = TestEnum.Test1;
}

public enum TestEnum
{
    Test1 = 1,
    Test2 = 2,
    Test3 = 3,
    Test4 = 4,
    Test5 = 5,
    Test6 = 6,
    Test7 = 7,
    Test8 = 8,
}