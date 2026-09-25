using Lagrange.Proto.Serialization;

namespace Lagrange.Proto.Test;

[TestFixture]
public class UnknownTest
{
    private Test _test;
    
    private byte[] _bytes;
    
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
            Test13 = true
        };
        
        _test = test;
        _bytes = ProtoSerializer.Serialize(test);
    }
    
    [Test]
    public void TestSrcGen()
    {
        var obj = ProtoSerializer.DeserializeProtoPackable<TestParts>(_bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(obj.Test1, Is.EqualTo(_test.Test1));
        });
    }
}

[ProtoPackable]
public partial class TestParts
{
    [ProtoMember(1, NumberHandling = ProtoNumberHandling.Signed)] 
    public int Test1 { get; set; }
}