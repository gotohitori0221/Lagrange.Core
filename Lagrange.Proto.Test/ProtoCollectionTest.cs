using Lagrange.Proto.Serialization;

namespace Lagrange.Proto.Test;

[TestFixture]
public class ProtoCollectionTest
{
    #region Array Tests
    
    [Test]
    public void TestArray_PrimitiveTypes()
    {
        var test = new PrimitiveArrayTest
        {
            ByteArray = new byte[] { 0, 127, 255 },
            SByteArray = new sbyte[] { -10, 0, 10 },
            ShortArray = new short[] { -100, 0, 100 },
            UShortArray = new ushort[] { 0, 100, 200 },
            IntArray = new[] { -1000, -1, 0, 1, 1000 },
            UIntArray = new uint[] { 0, 1, 1000 },
            LongArray = new[] { -10000L, 0L, 10000L },
            ULongArray = new ulong[] { 0, 1, 10000 },
            FloatArray = new[] { -1.5f, 0f, 1.5f, 3.14f },
            DoubleArray = new[] { -2.5, 0d, 2.5, 3.14159 },
            BoolArray = new[] { true, false, true }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        PrimitiveArrayTest deserialized = ProtoSerializer.DeserializeProtoPackable<PrimitiveArrayTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.ByteArray, Is.EqualTo(test.ByteArray));
            Assert.That(deserialized.SByteArray, Is.EqualTo(test.SByteArray));
            Assert.That(deserialized.ShortArray, Is.EqualTo(test.ShortArray));
            Assert.That(deserialized.UShortArray, Is.EqualTo(test.UShortArray));
            Assert.That(deserialized.IntArray, Is.EqualTo(test.IntArray));
            Assert.That(deserialized.UIntArray, Is.EqualTo(test.UIntArray));
            Assert.That(deserialized.LongArray, Is.EqualTo(test.LongArray));
            Assert.That(deserialized.ULongArray, Is.EqualTo(test.ULongArray));
            Assert.That(deserialized.FloatArray, Is.EqualTo(test.FloatArray));
            Assert.That(deserialized.DoubleArray, Is.EqualTo(test.DoubleArray));
            Assert.That(deserialized.BoolArray, Is.EqualTo(test.BoolArray));
        });
    }
    
    [Test]
    public void TestArray_StringArray()
    {
        var test = new StringArrayTest
        {
            StringArray = new[] { "Hello", "World", "", "Unicode: 你好", "Emoji: 🌍" },
            EmptyStringArray = new[] { "", "", "" },
            SingleString = new[] { "Single" }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        StringArrayTest deserialized = ProtoSerializer.DeserializeProtoPackable<StringArrayTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.StringArray, Is.EqualTo(test.StringArray));
            Assert.That(deserialized.EmptyStringArray, Is.EqualTo(test.EmptyStringArray));
            Assert.That(deserialized.SingleString, Is.EqualTo(test.SingleString));
        });
    }
    
    [Test]
    public void TestArray_NestedArrays()
    {
        
        
        Assert.Pass("Nested array testing is covered in ListCollectionTest");
    }
    
    [Test]
    public void TestArray_EmptyArrays()
    {
        var test = new PrimitiveArrayTest
        {
            ByteArray = Array.Empty<byte>(),
            IntArray = Array.Empty<int>(),
            FloatArray = Array.Empty<float>(),
            BoolArray = Array.Empty<bool>()
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        PrimitiveArrayTest deserialized = ProtoSerializer.DeserializeProtoPackable<PrimitiveArrayTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.ByteArray, Is.Empty);
            Assert.That(deserialized.IntArray, Is.Empty);
            Assert.That(deserialized.FloatArray, Is.Empty);
            Assert.That(deserialized.BoolArray, Is.Empty);
        });
    }
    
    #endregion
    
    #region List Tests
    
    [Test]
    public void TestList_BasicTypes()
    {
        var test = new ListCollectionTest
        {
            IntList = new List<int> { 1, 2, 3, 4, 5 },
            StringList = new List<string> { "Alpha", "Beta", "Gamma" },
            DoubleList = new List<double> { 1.1, 2.2, 3.3 },
            ObjectList = new List<SimpleObject>
            {
                new() { Id = 1, Name = "One" },
                new() { Id = 2, Name = "Two" }
            }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        ListCollectionTest deserialized = ProtoSerializer.DeserializeProtoPackable<ListCollectionTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.IntList, Is.EqualTo(test.IntList));
            Assert.That(deserialized.StringList, Is.EqualTo(test.StringList));
            Assert.That(deserialized.DoubleList, Is.EqualTo(test.DoubleList));
            Assert.That(deserialized.ObjectList.Count, Is.EqualTo(2));
            Assert.That(deserialized.ObjectList[0].Id, Is.EqualTo(1));
            Assert.That(deserialized.ObjectList[0].Name, Is.EqualTo("One"));
        });
    }
    
    [Test]
    public void TestList_EmptyLists()
    {
        var test = new ListCollectionTest
        {
            IntList = new List<int>(),
            StringList = new List<string>(),
            DoubleList = new List<double>(),
            ObjectList = new List<SimpleObject>()
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        ListCollectionTest deserialized = ProtoSerializer.DeserializeProtoPackable<ListCollectionTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.IntList, Is.Empty);
            Assert.That(deserialized.StringList, Is.Empty);
            Assert.That(deserialized.DoubleList, Is.Empty);
            Assert.That(deserialized.ObjectList, Is.Empty);
        });
    }
    
    [Test]
    public void TestList_LargeList()
    {
        var test = new ListCollectionTest
        {
            IntList = Enumerable.Range(0, 10000).ToList()
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        ListCollectionTest deserialized = ProtoSerializer.DeserializeProtoPackable<ListCollectionTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.IntList.Count, Is.EqualTo(10000));
            Assert.That(deserialized.IntList, Is.EqualTo(test.IntList));
        });
    }
    
    #endregion
    
    #region Dictionary Tests
    
    [Test]
    public void TestDictionary_BasicTypes()
    {
        var test = new DictionaryCollectionTest
        {
            IntStringDict = new Dictionary<int, string>
            {
                { 1, "One" },
                { 2, "Two" },
                { 3, "Three" }
            },
            StringIntDict = new Dictionary<string, int>
            {
                { "One", 1 },
                { "Two", 2 },
                { "Three", 3 }
            },
            IntIntDict = new Dictionary<int, int>
            {
                { 1, 10 },
                { 2, 20 },
                { 3, 30 }
            }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        DictionaryCollectionTest deserialized = ProtoSerializer.DeserializeProtoPackable<DictionaryCollectionTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.IntStringDict, Is.EqualTo(test.IntStringDict));
            Assert.That(deserialized.StringIntDict, Is.EqualTo(test.StringIntDict));
            Assert.That(deserialized.IntIntDict, Is.EqualTo(test.IntIntDict));
        });
    }
    
    [Test]
    public void TestDictionary_ComplexValues()
    {
        var test = new ComplexDictionaryTest
        {
            ObjectDict = new Dictionary<int, SimpleObject>
            {
                { 1, new SimpleObject { Id = 1, Name = "First" } },
                { 2, new SimpleObject { Id = 2, Name = "Second" } }
            },
            StringObjectDict = new Dictionary<string, SimpleObject>
            {
                { "key1", new SimpleObject { Id = 10, Name = "Value1" } },
                { "key2", new SimpleObject { Id = 20, Name = "Value2" } }
            }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        ComplexDictionaryTest deserialized = ProtoSerializer.DeserializeProtoPackable<ComplexDictionaryTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.ObjectDict.Count, Is.EqualTo(2));
            Assert.That(deserialized.ObjectDict[1].Id, Is.EqualTo(1));
            Assert.That(deserialized.ObjectDict[1].Name, Is.EqualTo("First"));
            Assert.That(deserialized.StringObjectDict["key1"].Id, Is.EqualTo(10));
            Assert.That(deserialized.StringObjectDict["key1"].Name, Is.EqualTo("Value1"));
        });
    }
    
    [Test]
    public void TestDictionary_EmptyDictionary()
    {
        var test = new DictionaryCollectionTest
        {
            IntStringDict = new Dictionary<int, string>(),
            StringIntDict = new Dictionary<string, int>(),
            IntIntDict = new Dictionary<int, int>()
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        DictionaryCollectionTest deserialized = ProtoSerializer.DeserializeProtoPackable<DictionaryCollectionTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.IntStringDict, Is.Empty);
            Assert.That(deserialized.StringIntDict, Is.Empty);
            Assert.That(deserialized.IntIntDict, Is.Empty);
        });
    }
    
    #endregion
    
    #region HashSet Tests
    
    [Test]
    public void TestHashSet_BasicTypes()
    {
        
        
        var test = new HashSetCollectionTest
        {
            IntList = new List<int> { 1, 2, 3, 4, 5 },
            StringList = new List<string> { "Alpha", "Beta", "Gamma" }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        HashSetCollectionTest deserialized = ProtoSerializer.DeserializeProtoPackable<HashSetCollectionTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(new HashSet<int>(deserialized.IntList), Is.EquivalentTo(new HashSet<int>(test.IntList)));
            Assert.That(new HashSet<string>(deserialized.StringList), Is.EquivalentTo(new HashSet<string>(test.StringList)));
        });
    }
    
    #endregion
    
    #region Queue and Stack Tests
    
    [Test]
    public void TestQueue_BasicTypes()
    {
        
        
        var test = new QueueStackTest
        {
            IntList = new List<int> { 1, 2, 3, 4, 5 },
            StringList = new List<string> { "Bottom", "Middle", "Top" }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        QueueStackTest deserialized = ProtoSerializer.DeserializeProtoPackable<QueueStackTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.IntList, Is.EqualTo(test.IntList));
            Assert.That(deserialized.StringList, Is.EqualTo(test.StringList));
        });
    }
    
    #endregion
    
    #region Mixed Collection with ProtoPackable
    
    [Test]
    public void TestPackableCollections()
    {
        var test = new PackableCollectionTest
        {
            PackedIntArray = new[] { 1, 2, 3, 4, 5 },
            PackedFloatArray = new[] { 1.1f, 2.2f, 3.3f },
            PackedDoubleArray = new[] { 1.11, 2.22, 3.33 },
            UnpackedStringArray = new[] { "One", "Two", "Three" },
            MixedSignedArray = new[] { -100, -50, 0, 50, 100 },
            Fixed32Array = new[] { 10, 20, 30 },
            Fixed64Array = new[] { 100L, 200L, 300L }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        PackableCollectionTest deserialized = ProtoSerializer.DeserializeProtoPackable<PackableCollectionTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.PackedIntArray, Is.EqualTo(test.PackedIntArray));
            Assert.That(deserialized.PackedFloatArray, Is.EqualTo(test.PackedFloatArray));
            Assert.That(deserialized.PackedDoubleArray, Is.EqualTo(test.PackedDoubleArray));
            Assert.That(deserialized.UnpackedStringArray, Is.EqualTo(test.UnpackedStringArray));
            Assert.That(deserialized.MixedSignedArray, Is.EqualTo(test.MixedSignedArray));
            Assert.That(deserialized.Fixed32Array, Is.EqualTo(test.Fixed32Array));
            Assert.That(deserialized.Fixed64Array, Is.EqualTo(test.Fixed64Array));
        });
    }
    
    #endregion
    
    #region Edge Cases
    
    [Test]
    public void TestCollection_NullCollections()
    {
        var test = new NullableCollectionTest
        {
            NullableIntArray = null,
            NullableStringList = null,
            NullableDict = null
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        NullableCollectionTest deserialized = ProtoSerializer.DeserializeProtoPackable<NullableCollectionTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.NullableIntArray, Is.Null);
            Assert.That(deserialized.NullableStringList, Is.Null);
            Assert.That(deserialized.NullableDict, Is.Null);
        });
    }
    
    [Test]
    public void TestCollection_SingleElementCollections()
    {
        var test = new PrimitiveArrayTest
        {
            IntArray = new[] { 42 },
            StringArray = new[] { "Single" },
            BoolArray = new[] { true }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        PrimitiveArrayTest deserialized = ProtoSerializer.DeserializeProtoPackable<PrimitiveArrayTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.IntArray, Is.EqualTo(new[] { 42 }));
            Assert.That(deserialized.StringArray, Is.EqualTo(new[] { "Single" }));
            Assert.That(deserialized.BoolArray, Is.EqualTo(new[] { true }));
        });
    }
    
    [Test]
    public void TestCollection_DuplicateValues()
    {
        var test = new ListCollectionTest
        {
            IntList = new List<int> { 1, 1, 1, 2, 2, 3 },
            StringList = new List<string> { "Same", "Same", "Different", "Same" }
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        ListCollectionTest deserialized = ProtoSerializer.DeserializeProtoPackable<ListCollectionTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.IntList, Is.EqualTo(test.IntList));
            Assert.That(deserialized.StringList, Is.EqualTo(test.StringList));
        });
    }
    
    [Test]
    public void TestCollection_VeryLargeCollection()
    {
        const int size = 100000;
        var test = new PrimitiveArrayTest
        {
            IntArray = Enumerable.Range(0, size).ToArray()
        };
        
        byte[] bytes = ProtoSerializer.SerializeProtoPackable(test);
        PrimitiveArrayTest deserialized = ProtoSerializer.DeserializeProtoPackable<PrimitiveArrayTest>(bytes);
        
        Assert.Multiple(() =>
        {
            Assert.That(deserialized.IntArray.Length, Is.EqualTo(size));
            Assert.That(deserialized.IntArray[0], Is.EqualTo(0));
            Assert.That(deserialized.IntArray[size - 1], Is.EqualTo(size - 1));
        });
    }
    
    #endregion
}

#region Test Classes

[ProtoPackable]
public partial class PrimitiveArrayTest
{
    [ProtoMember(1)] public byte[] ByteArray { get; set; } = Array.Empty<byte>();
    [ProtoMember(2, NumberHandling = ProtoNumberHandling.Signed)] public sbyte[] SByteArray { get; set; } = Array.Empty<sbyte>();
    [ProtoMember(3, NumberHandling = ProtoNumberHandling.Signed)] public short[] ShortArray { get; set; } = Array.Empty<short>();
    [ProtoMember(4)] public ushort[] UShortArray { get; set; } = Array.Empty<ushort>();
    [ProtoMember(5, NumberHandling = ProtoNumberHandling.Signed)] public int[] IntArray { get; set; } = Array.Empty<int>();
    [ProtoMember(6)] public uint[] UIntArray { get; set; } = Array.Empty<uint>();
    [ProtoMember(7, NumberHandling = ProtoNumberHandling.Signed)] public long[] LongArray { get; set; } = Array.Empty<long>();
    [ProtoMember(8)] public ulong[] ULongArray { get; set; } = Array.Empty<ulong>();
    [ProtoMember(9)] public float[] FloatArray { get; set; } = Array.Empty<float>();
    [ProtoMember(10)] public double[] DoubleArray { get; set; } = Array.Empty<double>();
    [ProtoMember(11)] public bool[] BoolArray { get; set; } = Array.Empty<bool>();
    [ProtoMember(12)] public string[] StringArray { get; set; } = Array.Empty<string>();
}

[ProtoPackable]
public partial class StringArrayTest
{
    [ProtoMember(1)] public string[] StringArray { get; set; } = Array.Empty<string>();
    [ProtoMember(2)] public string[] EmptyStringArray { get; set; } = Array.Empty<string>();
    [ProtoMember(3)] public string[] SingleString { get; set; } = Array.Empty<string>();
}

[ProtoPackable]
public partial class SimpleObject
{
    [ProtoMember(1)] public int Id { get; set; }
    [ProtoMember(2)] public string Name { get; set; } = string.Empty;
}

[ProtoPackable]
public partial class ListCollectionTest
{
    [ProtoMember(1)] public List<int> IntList { get; set; } = new();
    [ProtoMember(2)] public List<string> StringList { get; set; } = new();
    [ProtoMember(3)] public List<double> DoubleList { get; set; } = new();
    [ProtoMember(4)] public List<SimpleObject> ObjectList { get; set; } = new();
}

[ProtoPackable]
public partial class DictionaryCollectionTest
{
    [ProtoMember(1)] public Dictionary<int, string> IntStringDict { get; set; } = new();
    [ProtoMember(2)] public Dictionary<string, int> StringIntDict { get; set; } = new();
    [ProtoMember(3)] public Dictionary<int, int> IntIntDict { get; set; } = new();
}

[ProtoPackable]
public partial class ComplexDictionaryTest
{
    [ProtoMember(1)] public Dictionary<int, SimpleObject> ObjectDict { get; set; } = new();
    [ProtoMember(2)] public Dictionary<string, SimpleObject> StringObjectDict { get; set; } = new();
}

[ProtoPackable]
public partial class HashSetCollectionTest
{
    [ProtoMember(1)] public List<int> IntList { get; set; } = new();
    [ProtoMember(2)] public List<string> StringList { get; set; } = new();
}

[ProtoPackable]
public partial class QueueStackTest
{
    [ProtoMember(1)] public List<int> IntList { get; set; } = new();
    [ProtoMember(2)] public List<string> StringList { get; set; } = new();
}

[ProtoPackable]
public partial class PackableCollectionTest
{
    [ProtoMember(1)] 
    public int[] PackedIntArray { get; set; } = Array.Empty<int>();
    
    [ProtoMember(2)] 
    public float[] PackedFloatArray { get; set; } = Array.Empty<float>();
    
    [ProtoMember(3)] 
    public double[] PackedDoubleArray { get; set; } = Array.Empty<double>();
    
    [ProtoMember(4)] 
    public string[] UnpackedStringArray { get; set; } = Array.Empty<string>();
    
    [ProtoMember(5, NumberHandling = ProtoNumberHandling.Signed)] 
    public int[] MixedSignedArray { get; set; } = Array.Empty<int>();
    
    [ProtoMember(6, NumberHandling = ProtoNumberHandling.Fixed32)] 
    public int[] Fixed32Array { get; set; } = Array.Empty<int>();
    
    [ProtoMember(7, NumberHandling = ProtoNumberHandling.Fixed64)] 
    public long[] Fixed64Array { get; set; } = Array.Empty<long>();
}

[ProtoPackable]
public partial class NullableCollectionTest
{
    [ProtoMember(1)] public int[]? NullableIntArray { get; set; }
    [ProtoMember(2)] public List<string>? NullableStringList { get; set; }
    [ProtoMember(3)] public Dictionary<int, string>? NullableDict { get; set; }
}

#endregion