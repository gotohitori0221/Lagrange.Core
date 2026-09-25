using Lagrange.Proto.Serialization;

namespace Lagrange.Proto.Test;

[TestFixture]
public class CollectionTest
{
    private TestCollection _test;
    
    [SetUp]
    public void Setup()
    {
        _test = new TestCollection
        {
            Test1 = new[] { 1, 2, 3 },
            Test2 = new[] { "Test", "Test2" },
            Test3 = new[] { 1.0f, 2.0f, 3.0f },
            Test4 = new TestCollection2
            {
                Test1 = new Dictionary<int, string>
                {
                    { 1, "Test" },
                    { 2, "Test2" }
                },
                Test2 = new Dictionary<int, int>
                {
                    { 1, 1 },
                    { 2, 2 }
                }
            }
        };
    }

    [Test]
    public void TestSerializeProtoPackable()
    {
        var bytes = ProtoSerializer.SerializeProtoPackable(_test);
        var test2 = ProtoSerializer.DeserializeProtoPackable<TestCollection>(bytes);

        Assert.Multiple(() =>
        {
            Assert.That(test2.Test1, Is.EqualTo(_test.Test1));
            Assert.That(test2.Test2, Is.EqualTo(_test.Test2));
            Assert.That(test2.Test3, Is.EqualTo(_test.Test3));
            Assert.That(test2.Test4.Test1, Is.EqualTo(_test.Test4.Test1));
            Assert.That(test2.Test4.Test2, Is.EqualTo(_test.Test4.Test2));
        });
    }
    
    [Test]
    public void TestSerializeProto()
    {
        var bytes = ProtoSerializer.Serialize(_test);
        var test2 = ProtoSerializer.Deserialize<TestCollection>(bytes);

        Assert.Multiple(() =>
        {
            Assert.That(test2.Test1, Is.EqualTo(_test.Test1));
            Assert.That(test2.Test2, Is.EqualTo(_test.Test2));
            Assert.That(test2.Test3, Is.EqualTo(_test.Test3));
            Assert.That(test2.Test4.Test1, Is.EqualTo(_test.Test4.Test1));
            Assert.That(test2.Test4.Test2, Is.EqualTo(_test.Test4.Test2));
        });
    }
}

[ProtoPackable]
public partial class TestCollection
{
    [ProtoMember(1, NumberHandling = ProtoNumberHandling.Signed | ProtoNumberHandling.Fixed64)] 
    public int[] Test1 { get; set; } = [];
    
    [ProtoMember(2)] public string[] Test2 { get; set; } = [];
    
    [ProtoMember(3)] public float[] Test3 { get; set; } = [];
    
    [ProtoMember(4)] public TestCollection2 Test4 { get; set; } = new();
}

[ProtoPackable]
public partial class TestCollection2
{
    [ProtoMember(1)] public Dictionary<int, string> Test1 { get; set; } = new();
    
    [ProtoMember(2)] [ProtoValueMember(NumberHandling = ProtoNumberHandling.Signed)]
    public Dictionary<int, int> Test2 { get; set; } = new();
}