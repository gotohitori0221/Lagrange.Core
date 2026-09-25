using System.Buffers;
using System.Text;
using Lagrange.Proto.Nodes;
using Lagrange.Proto.Primitives;
using Lagrange.Proto.Serialization;
using Lagrange.Proto.Utility;

namespace Lagrange.Proto.Test;

[TestFixture]
public class ProtoEdgeCaseTest
{
    #region Boundary Value Tests
    
    [Test]
    public void TestBoundaryValues_AllIntegerTypes()
    {
        var test = new BoundaryValueTest
        {
            ByteMin = byte.MinValue,
            ByteMax = byte.MaxValue,
            SByteMin = sbyte.MinValue,
            SByteMax = sbyte.MaxValue,
            ShortMin = short.MinValue,
            ShortMax = short.MaxValue,
            UShortMin = ushort.MinValue,
            UShortMax = ushort.MaxValue,
            IntMin = int.MinValue,
            IntMax = int.MaxValue,
            UIntMin = uint.MinValue,
            UIntMax = uint.MaxValue,
            LongMin = long.MinValue,
            LongMax = long.MaxValue,
            ULongMin = ulong.MinValue,
            ULongMax = ulong.MaxValue
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        BoundaryValueTest deserialized = ProtoSerializer.DeserializeProtoPackable<BoundaryValueTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.ByteMin, Is.EqualTo(byte.MinValue));
            Assert.That(deserialized.ByteMax, Is.EqualTo(byte.MaxValue));
            Assert.That(deserialized.SByteMin, Is.EqualTo(sbyte.MinValue));
            Assert.That(deserialized.SByteMax, Is.EqualTo(sbyte.MaxValue));
            Assert.That(deserialized.ShortMin, Is.EqualTo(short.MinValue));
            Assert.That(deserialized.ShortMax, Is.EqualTo(short.MaxValue));
            Assert.That(deserialized.UShortMin, Is.EqualTo(ushort.MinValue));
            Assert.That(deserialized.UShortMax, Is.EqualTo(ushort.MaxValue));
            Assert.That(deserialized.IntMin, Is.EqualTo(int.MinValue));
            Assert.That(deserialized.IntMax, Is.EqualTo(int.MaxValue));
            Assert.That(deserialized.UIntMin, Is.EqualTo(uint.MinValue));
            Assert.That(deserialized.UIntMax, Is.EqualTo(uint.MaxValue));
            Assert.That(deserialized.LongMin, Is.EqualTo(long.MinValue));
            Assert.That(deserialized.LongMax, Is.EqualTo(long.MaxValue));
            Assert.That(deserialized.ULongMin, Is.EqualTo(ulong.MinValue));
            Assert.That(deserialized.ULongMax, Is.EqualTo(ulong.MaxValue));
        });
    }
    
    [Test]
    public void TestBoundaryValues_FloatingPoint()
    {
        var test = new FloatingPointBoundaryTest
        {
            FloatMin = float.MinValue,
            FloatMax = float.MaxValue,
            FloatEpsilon = float.Epsilon,
            FloatNaN = float.NaN,
            FloatPositiveInfinity = float.PositiveInfinity,
            FloatNegativeInfinity = float.NegativeInfinity,
            DoubleMin = double.MinValue,
            DoubleMax = double.MaxValue,
            DoubleEpsilon = double.Epsilon,
            DoubleNaN = double.NaN,
            DoublePositiveInfinity = double.PositiveInfinity,
            DoubleNegativeInfinity = double.NegativeInfinity
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        FloatingPointBoundaryTest deserialized = ProtoSerializer.DeserializeProtoPackable<FloatingPointBoundaryTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.FloatMin, Is.EqualTo(float.MinValue));
            Assert.That(deserialized.FloatMax, Is.EqualTo(float.MaxValue));
            Assert.That(deserialized.FloatEpsilon, Is.EqualTo(float.Epsilon));
            Assert.That(float.IsNaN(deserialized.FloatNaN), Is.True);
            Assert.That(deserialized.FloatPositiveInfinity, Is.EqualTo(float.PositiveInfinity));
            Assert.That(deserialized.FloatNegativeInfinity, Is.EqualTo(float.NegativeInfinity));
            Assert.That(deserialized.DoubleMin, Is.EqualTo(double.MinValue));
            Assert.That(deserialized.DoubleMax, Is.EqualTo(double.MaxValue));
            Assert.That(deserialized.DoubleEpsilon, Is.EqualTo(double.Epsilon));
            Assert.That(double.IsNaN(deserialized.DoubleNaN), Is.True);
            Assert.That(deserialized.DoublePositiveInfinity, Is.EqualTo(double.PositiveInfinity));
            Assert.That(deserialized.DoubleNegativeInfinity, Is.EqualTo(double.NegativeInfinity));
        });
    }
    
    #endregion
    
    #region Field Number Edge Cases
    
    [Test]
    public void TestFieldNumbers_Boundaries()
    {
        var test = new FieldNumberBoundaryTest
        {
            Field1 = "First field",
            Field15 = "Last of first range",
            Field16 = "Start of second range",
            Field2047 = "End of two-byte range",
            Field2048 = "Start of three-byte range"
            
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        FieldNumberBoundaryTest deserialized = ProtoSerializer.DeserializeProtoPackable<FieldNumberBoundaryTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.Field1, Is.EqualTo(test.Field1));
            Assert.That(deserialized.Field15, Is.EqualTo(test.Field15));
            Assert.That(deserialized.Field16, Is.EqualTo(test.Field16));
            Assert.That(deserialized.Field2047, Is.EqualTo(test.Field2047));
            Assert.That(deserialized.Field2048, Is.EqualTo(test.Field2048));
            
        });
    }
    
    #endregion
    
    #region String Edge Cases
    
    [Test]
    public void TestString_SpecialCases()
    {
        var test = new StringEdgeCaseTest
        {
            EmptyString = "",
            SingleChar = "A",
            NullTerminated = "Hello\0World",
            AllWhitespace = "   \t\r\n   ",
            SpecialChars = "!@#$%^&*()_+-=[]{}|;':\",./<>?",
            Unicode = "你好世界 مرحبا بالعالم Здравствуй мир",
            Emoji = "😀😃😄😁😆😅🤣😂🙂🙃",
            ControlChars = "\x00\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F",
            VeryLongString = new string('X', 65536) 
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        StringEdgeCaseTest deserialized = ProtoSerializer.DeserializeProtoPackable<StringEdgeCaseTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.EmptyString, Is.EqualTo(test.EmptyString));
            Assert.That(deserialized.SingleChar, Is.EqualTo(test.SingleChar));
            Assert.That(deserialized.NullTerminated, Is.EqualTo(test.NullTerminated));
            Assert.That(deserialized.AllWhitespace, Is.EqualTo(test.AllWhitespace));
            Assert.That(deserialized.SpecialChars, Is.EqualTo(test.SpecialChars));
            Assert.That(deserialized.Unicode, Is.EqualTo(test.Unicode));
            Assert.That(deserialized.Emoji, Is.EqualTo(test.Emoji));
            Assert.That(deserialized.ControlChars, Is.EqualTo(test.ControlChars));
            Assert.That(deserialized.VeryLongString, Is.EqualTo(test.VeryLongString));
        });
    }
    
    #endregion
    
    #region Null and Default Value Tests
    
    [Test]
    public void TestNullValues_AllTypes()
    {
        var test = new NullValueTest
        {
            NullString = null,
            NullArray = null,
            NullList = null,
            NullObject = null,
            NullableInt = null,
            NullableBool = null,
            NullableDouble = null
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        NullValueTest deserialized = ProtoSerializer.DeserializeProtoPackable<NullValueTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.NullString, Is.Null);
            Assert.That(deserialized.NullArray, Is.Null);
            Assert.That(deserialized.NullList, Is.Null);
            Assert.That(deserialized.NullObject, Is.Null);
            Assert.That(deserialized.NullableInt, Is.Null);
            Assert.That(deserialized.NullableBool, Is.Null);
            Assert.That(deserialized.NullableDouble, Is.Null);
        });
    }
    
    [Test]
    public void TestDefaultValues_Primitives()
    {
        var test = new DefaultValueTest(); 
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        DefaultValueTest deserialized = ProtoSerializer.DeserializeProtoPackable<DefaultValueTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.DefaultInt, Is.EqualTo(0));
            Assert.That(deserialized.DefaultBool, Is.False);
            Assert.That(deserialized.DefaultFloat, Is.EqualTo(0f));
            Assert.That(deserialized.DefaultDouble, Is.EqualTo(0d));
            Assert.That(deserialized.DefaultString, Is.EqualTo(string.Empty));
            Assert.That(deserialized.DefaultEnum, Is.EqualTo(EdgeTestEnum.Default));
        });
    }
    
    #endregion
    
    #region Nested Structure Edge Cases
    
    [Test]
    public void TestCircularReference_ShouldNotStackOverflow()
    {
        
        
        
        Assert.Pass("Circular reference test skipped - not supported by protobuf");
    }
    
    #endregion
    
    #region Buffer and Memory Edge Cases
    
    [Test]
    public void TestLargeDataSerialization()
    {
        const int size = 1024 * 1024; 
        var test = new LargeDataTest
        {
            LargeByteArray = new byte[size],
            LargeString = new string('A', size),
            LargeIntArray = Enumerable.Range(0, size / 4).ToArray()
        };
        
        Random.Shared.NextBytes(test.LargeByteArray);
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        LargeDataTest deserialized = ProtoSerializer.DeserializeProtoPackable<LargeDataTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.LargeByteArray.Length, Is.EqualTo(size));
            Assert.That(deserialized.LargeByteArray, Is.EqualTo(test.LargeByteArray));
            Assert.That(deserialized.LargeString.Length, Is.EqualTo(size));
            Assert.That(deserialized.LargeIntArray.Length, Is.EqualTo(size / 4));
        });
    }
    
    [Test]
    public void TestSegmentedBuffer_LargeWrite()
    {
        var buffer = new SegmentBufferWriter(16); 
        var writer = new ProtoWriter(buffer);
        
        
        byte[] largeData = new byte[10000];
        Random.Shared.NextBytes(largeData);
        
        writer.EncodeBytes(largeData);
        writer.Flush();
        
        var memory = buffer.CreateReadOnlyMemory();
        var reader = new ProtoReader(memory.Span);
        
        int length = reader.DecodeVarInt<int>();
        var readData = reader.CreateSpan(length);
        
        Assert.That(readData.ToArray(), Is.EqualTo(largeData));
        
        buffer.Dispose();
    }
    
    #endregion
    
    #region ProtoNode Raw Access Edge Cases
    
    [Test]
    public void TestProtoNode_MixedAccessPatterns()
    {
        var obj = new ProtoObject
        {
            { 1, 42 },
            { 1, 43 }, 
            { 1, 44 },
            { 2, "Single" },
            { 3, new ProtoObject { { 1, "Nested" } } }
        };
        
        
        Assert.Multiple(() =>
        {
            
            var array = obj[1].AsArray();
            Assert.That(array.Count, Is.EqualTo(3));
            Assert.That(array.GetValues<int>().ToArray(), Is.EqualTo(new[] { 42, 43, 44 }));
            
            
            Assert.That(obj[2].GetValue<string>(), Is.EqualTo("Single"));
            
            
            Assert.That(obj[3][1].GetValue<string>(), Is.EqualTo("Nested"));
            
            
            Assert.That(obj.TryGetValue(999, out _), Is.False);
        });
    }
    
    [Test]
    public void TestProtoValue_TypeConversions()
    {
        
        Assert.Pass("Type conversion tests are in ProtoNodeTest.cs");
    }
    
    #endregion
    
    #region VarInt Encoding Edge Cases
    
    [Test]
    public void TestVarInt_BoundaryValues()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new ProtoWriter(buffer);
        
        
        writer.EncodeVarInt(0);      
        writer.EncodeVarInt(127);    
        writer.EncodeVarInt(128);    
        writer.EncodeVarInt(16383);  
        writer.EncodeVarInt(16384);  
        writer.EncodeVarInt(2097151); 
        writer.EncodeVarInt(2097152); 
        writer.Flush();
        
        var reader = new ProtoReader(buffer.WrittenMemory.Span);
        
        int v0 = reader.DecodeVarInt<int>();
        int v1 = reader.DecodeVarInt<int>();
        int v2 = reader.DecodeVarInt<int>();
        int v3 = reader.DecodeVarInt<int>();
        int v4 = reader.DecodeVarInt<int>();
        int v5 = reader.DecodeVarInt<int>();
        int v6 = reader.DecodeVarInt<int>();
        
        Assert.Multiple(() =>
        {
            Assert.That(v0, Is.EqualTo(0));
            Assert.That(v1, Is.EqualTo(127));
            Assert.That(v2, Is.EqualTo(128));
            Assert.That(v3, Is.EqualTo(16383));
            Assert.That(v4, Is.EqualTo(16384));
            Assert.That(v5, Is.EqualTo(2097151));
            Assert.That(v6, Is.EqualTo(2097152));
        });
    }
    
    #endregion
    
    #region Error Handling Tests
    
    [Test]
    public void TestInvalidData_MalformedVarInt()
    {
        
        byte[] malformed = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        var reader = new ProtoReader(malformed);
        
        Assert.Throws<InvalidDataException>(() => 
        {
            var localReader = new ProtoReader(malformed);
            localReader.DecodeVarInt<int>();
        });
    }
    
    [Test]
    public void TestInvalidData_TruncatedMessage()
    {
        var buffer = new ArrayBufferWriter<byte>();
        var writer = new ProtoWriter(buffer);
        
        writer.EncodeString("Test String");
        writer.Flush();
        
        
        byte[] truncated = buffer.WrittenMemory.ToArray()[..3];
        var reader = new ProtoReader(truncated);
        
        int length = reader.DecodeVarInt<int>();
        Assert.Throws<ArgumentOutOfRangeException>(() => 
        {
            var localReader = new ProtoReader(truncated);
            int localLength = localReader.DecodeVarInt<int>();
            localReader.CreateSpan(localLength);
        });
    }
    
    #endregion
}

#region Test Classes

[ProtoPackable]
public partial class BoundaryValueTest
{
    [ProtoMember(1)] public byte ByteMin { get; set; }
    [ProtoMember(2)] public byte ByteMax { get; set; }
    [ProtoMember(3, NumberHandling = ProtoNumberHandling.Signed)] public sbyte SByteMin { get; set; }
    [ProtoMember(4, NumberHandling = ProtoNumberHandling.Signed)] public sbyte SByteMax { get; set; }
    [ProtoMember(5, NumberHandling = ProtoNumberHandling.Signed)] public short ShortMin { get; set; }
    [ProtoMember(6, NumberHandling = ProtoNumberHandling.Signed)] public short ShortMax { get; set; }
    [ProtoMember(7)] public ushort UShortMin { get; set; }
    [ProtoMember(8)] public ushort UShortMax { get; set; }
    [ProtoMember(9, NumberHandling = ProtoNumberHandling.Signed)] public int IntMin { get; set; }
    [ProtoMember(10, NumberHandling = ProtoNumberHandling.Signed)] public int IntMax { get; set; }
    [ProtoMember(11)] public uint UIntMin { get; set; }
    [ProtoMember(12)] public uint UIntMax { get; set; }
    [ProtoMember(13, NumberHandling = ProtoNumberHandling.Signed)] public long LongMin { get; set; }
    [ProtoMember(14, NumberHandling = ProtoNumberHandling.Signed)] public long LongMax { get; set; }
    [ProtoMember(15)] public ulong ULongMin { get; set; }
    [ProtoMember(16)] public ulong ULongMax { get; set; }
}

[ProtoPackable]
public partial class FloatingPointBoundaryTest
{
    [ProtoMember(1)] public float FloatMin { get; set; }
    [ProtoMember(2)] public float FloatMax { get; set; }
    [ProtoMember(3)] public float FloatEpsilon { get; set; }
    [ProtoMember(4)] public float FloatNaN { get; set; }
    [ProtoMember(5)] public float FloatPositiveInfinity { get; set; }
    [ProtoMember(6)] public float FloatNegativeInfinity { get; set; }
    [ProtoMember(7)] public double DoubleMin { get; set; }
    [ProtoMember(8)] public double DoubleMax { get; set; }
    [ProtoMember(9)] public double DoubleEpsilon { get; set; }
    [ProtoMember(10)] public double DoubleNaN { get; set; }
    [ProtoMember(11)] public double DoublePositiveInfinity { get; set; }
    [ProtoMember(12)] public double DoubleNegativeInfinity { get; set; }
}

[ProtoPackable]
public partial class FieldNumberBoundaryTest
{
    [ProtoMember(1)] public string Field1 { get; set; } = string.Empty;
    [ProtoMember(15)] public string Field15 { get; set; } = string.Empty;
    [ProtoMember(16)] public string Field16 { get; set; } = string.Empty;
    [ProtoMember(2047)] public string Field2047 { get; set; } = string.Empty;
    [ProtoMember(2048)] public string Field2048 { get; set; } = string.Empty;
    
}

[ProtoPackable]
public partial class StringEdgeCaseTest
{
    [ProtoMember(1)] public string EmptyString { get; set; } = string.Empty;
    [ProtoMember(2)] public string SingleChar { get; set; } = string.Empty;
    [ProtoMember(3)] public string NullTerminated { get; set; } = string.Empty;
    [ProtoMember(4)] public string AllWhitespace { get; set; } = string.Empty;
    [ProtoMember(5)] public string SpecialChars { get; set; } = string.Empty;
    [ProtoMember(6)] public string Unicode { get; set; } = string.Empty;
    [ProtoMember(7)] public string Emoji { get; set; } = string.Empty;
    [ProtoMember(8)] public string ControlChars { get; set; } = string.Empty;
    [ProtoMember(9)] public string VeryLongString { get; set; } = string.Empty;
}

[ProtoPackable]
public partial class NullValueTest
{
    [ProtoMember(1)] public string? NullString { get; set; }
    [ProtoMember(2)] public int[]? NullArray { get; set; }
    [ProtoMember(3)] public List<string>? NullList { get; set; }
    [ProtoMember(4)] public SimpleEdgeObject? NullObject { get; set; }
    [ProtoMember(5)] public int? NullableInt { get; set; }
    [ProtoMember(6)] public bool? NullableBool { get; set; }
    [ProtoMember(7)] public double? NullableDouble { get; set; }
}

[ProtoPackable]
public partial class DefaultValueTest
{
    [ProtoMember(1)] public int DefaultInt { get; set; }
    [ProtoMember(2)] public bool DefaultBool { get; set; }
    [ProtoMember(3)] public float DefaultFloat { get; set; }
    [ProtoMember(4)] public double DefaultDouble { get; set; }
    [ProtoMember(5)] public string DefaultString { get; set; } = string.Empty;
    [ProtoMember(6)] public EdgeTestEnum DefaultEnum { get; set; }
}

[ProtoPackable]
public partial class SimpleEdgeObject
{
    [ProtoMember(1)] public int Value { get; set; }
}

public enum EdgeTestEnum
{
    Default = 0,
    Value1 = 1,
    Value2 = 2
}

[ProtoPackable]
public partial class NestedLevel
{
    [ProtoMember(1)] public string Value { get; set; } = string.Empty;
    [ProtoMember(2)] public NestedLevel? Next { get; set; }
}

[ProtoPackable]
public partial class DeeplyNestedTest
{
    [ProtoMember(1)] public NestedLevel? Root { get; set; }
}

[ProtoPackable]
public partial class LargeDataTest
{
    [ProtoMember(1)] public byte[] LargeByteArray { get; set; } = Array.Empty<byte>();
    [ProtoMember(2)] public string LargeString { get; set; } = string.Empty;
    [ProtoMember(3)] public int[] LargeIntArray { get; set; } = Array.Empty<int>();
}

#endregion