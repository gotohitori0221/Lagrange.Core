using Lagrange.Proto.Serialization;

namespace Lagrange.Proto.Test;

[TestFixture]
public class ReflectionTest
{
    private TestClassReflection _test;
    
    [SetUp]
    public void Setup()
    {
        _test = new TestClassReflection
        {
            Test1 = 114514,
            Test2 = "Test",
            Test3 = 3.14f,
            Test4 = new TestClassReflectionSub
            {
                Test1 = 1,
                Test2 = "Test",
                Test3 = 3.14f
            }
        };
    }
    
    [Test]
    public void TestReflection()
    {
        var bytes = ProtoSerializer.Serialize(_test);
        var test2 = ProtoSerializer.Deserialize<TestClassReflection>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(test2.Test1, Is.EqualTo(_test.Test1));
            Assert.That(test2.Test2, Is.EqualTo(_test.Test2));
            Assert.That(test2.Test3, Is.EqualTo(_test.Test3));
            Assert.That(test2.Test4.Test1, Is.EqualTo(_test.Test4.Test1));
            Assert.That(test2.Test4.Test2, Is.EqualTo(_test.Test4.Test2));
            Assert.That(test2.Test4.Test3, Is.EqualTo(_test.Test4.Test3));
        });
    }
}

public class TestClassReflection
{
    [ProtoMember(1)] public int Test1 { get; set; } 
    
    [ProtoMember(2)] public string Test2 { get; set; } = string.Empty;
    
    [ProtoMember(3)] public float Test3 { get; set; }

    [ProtoMember(4)] public TestClassReflectionSub Test4 { get; set; } = new();
}

public class TestClassReflectionSub
{
    [ProtoMember(1)] public int Test1 { get; set; } 
    
    [ProtoMember(2)] public string Test2 { get; set; } = string.Empty;
    
    [ProtoMember(3)] public float Test3 { get; set; }
}
