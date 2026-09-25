using System.Buffers.Binary;
using System.Text;
using Lagrange.Core.Utility.Binary;

namespace Lagrange.Core.Test.Binary;

[Parallelizable]
public class BinaryPacketTest
{
    private byte[] StackBuffer { get; set; }
    
    private byte[] ArrayPoolBuffer { get; set; }
    
    private byte[] PeekBuffer { get; set; }
    
    private Memory<byte> ReadOnlyMemoryBuffer { get; set; }
    
    private byte[] ExitLengthBarrierBuffer { get; set; }
    
    [SetUp]
    public void Setup()
    {
        var stackPacket = new BinaryPacket(stackalloc byte[200]);
        
        stackPacket.EnterLengthBarrier<int>();
        
        stackPacket.Write(1);
        stackPacket.Write(2);
        stackPacket.Write(3);
        stackPacket.Write(4u);
        stackPacket.Write(5L);
        stackPacket.Write(6ul);
        stackPacket.Write("Hello, World!", Prefix.Int16 | Prefix.WithPrefix);
        stackPacket.Write("Awoo!"u8, Prefix.Int32 | Prefix.WithPrefix);
        
        stackPacket.ExitLengthBarrier<int>(false);

        StackBuffer = stackPacket.ToArray();
        
        var arrayPoolPacket = new BinaryPacket(100);
        
        arrayPoolPacket.Write((short)1);
        arrayPoolPacket.Write(2);
        arrayPoolPacket.Write(3);
        arrayPoolPacket.Write((ushort)4);
        arrayPoolPacket.Write("Hello, World!"u8, Prefix.Int8 | Prefix.WithPrefix);
        arrayPoolPacket.Write("Awoo!".AsSpan(), Prefix.Int32 | Prefix.WithPrefix);
        arrayPoolPacket.Write(new byte[10000], Prefix.Int32 | Prefix.WithPrefix);
        arrayPoolPacket.Write("Test".AsSpan());
        
        ArrayPoolBuffer = arrayPoolPacket.ToArray();

        PeekBuffer = new byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(PeekBuffer, 0x123456789abcdef0);
        
        ReadOnlyMemoryBuffer = new byte[16];
        BinaryPrimitives.WriteUInt64BigEndian(ReadOnlyMemoryBuffer.Span, 0x123456789abcdef0);
        
        var exitLengthBarrierPacket = new BinaryPacket(200);
        exitLengthBarrierPacket.EnterLengthBarrier<int>();
        exitLengthBarrierPacket.ExitCustomBarrier(0x12345678);
        ExitLengthBarrierBuffer = exitLengthBarrierPacket.ToArray();
    }

    [Test]
    public void TestStackBuffer()
    {
        var packet = new BinaryPacket(StackBuffer.AsSpan());
        
        uint length = packet.Read<uint>();
        int value1 = packet.Read<int>();
        long value2 = packet.Read<long>();
        uint value3 = packet.Read<uint>();
        long value4 = packet.Read<long>();
        ulong value5 = packet.Read<ulong>();
        string value6 = Encoding.UTF8.GetString(packet.ReadBytes(Prefix.Int16 | Prefix.WithPrefix));
        string value7 = Encoding.UTF8.GetString(packet.ReadBytes(Prefix.Int32 | Prefix.WithPrefix));
        
        Assert.Multiple(() =>
        {
            Assert.That(length, Is.EqualTo(56));
            Assert.That(value1, Is.EqualTo(1));
            Assert.That(value2, Is.EqualTo(2ul << 32 | 3));
            Assert.That(value3, Is.EqualTo(4));
            Assert.That(value4, Is.EqualTo(5));
            Assert.That(value5, Is.EqualTo(6));
            Assert.That(value6, Is.EqualTo("Hello, World!"));
            Assert.That(value7, Is.EqualTo("Awoo!"));
        });

        Assert.Pass();
    }
    
    [Test]
    public void TestArrayPoolBuffer()
    {
        var packet = new BinaryPacket(ArrayPoolBuffer.AsSpan());
        
        short value1 = packet.Read<short>();
        long value2 = packet.Read<long>();
        ushort value3 = packet.Read<ushort>();
        string value4 = Encoding.UTF8.GetString(packet.ReadBytes(Prefix.Int8 | Prefix.WithPrefix));
        string value5 = Encoding.UTF8.GetString(packet.ReadBytes(Prefix.Int32 | Prefix.WithPrefix));
        var value6 = packet.ReadBytes(Prefix.Int32 | Prefix.WithPrefix).ToArray();
        
        Assert.Multiple(() =>
        {
            Assert.That(value1, Is.EqualTo(1));
            Assert.That(value2, Is.EqualTo(2ul << 32 | 3));
            Assert.That(value3, Is.EqualTo(4));
            Assert.That(value4, Is.EqualTo("Hello, World!"));
            Assert.That(value5, Is.EqualTo("Awoo!"));
            Assert.That(value6, Has.Length.EqualTo(10000));
        });

        Assert.Pass();
    }

    [Test]
    public void TestPeekBuffer()
    {
        var reader = new BinaryPacket(PeekBuffer.AsSpan());

        byte value1 = reader.Peek<byte>();
        short value2 = reader.Peek<short>();
        int value3 = reader.Peek<int>();
        long value4 = reader.Peek<long>();
        
        ushort value5 = reader.Peek<ushort>();
        uint value6 = reader.Peek<uint>();
        ulong value7 = reader.Peek<ulong>();
        
        reader.Skip(8);
        var span = reader.CreateReadOnlySpan();
        var bytes = span.ToArray();
        
        Assert.Multiple(() =>
        {
            Assert.That(value1, Is.EqualTo(0x12));
            Assert.That(value2, Is.EqualTo(0x1234));
            Assert.That(value3, Is.EqualTo(0x12345678));
            Assert.That(value4, Is.EqualTo(0x123456789abcdef0));
            
            Assert.That(value5, Is.EqualTo(0x1234));
            Assert.That(value6, Is.EqualTo(0x12345678));
            Assert.That(value7, Is.EqualTo(0x123456789abcdef0));
            
            Assert.That(bytes, Is.EqualTo(PeekBuffer));
        });
    }

    [Test]
    public void TestReadOnlyMemory()
    {
        var reader = new BinaryPacket(ReadOnlyMemoryBuffer);

        long value1 = reader.Read<long>();
        long value2 = reader.Read<long>();
        int length = reader.ToArray().Length;
        
        Assert.Multiple(() =>
        {
            Assert.That(value1, Is.EqualTo(0x123456789abcdef0));
            Assert.That(value2, Is.EqualTo(0));
            Assert.That(length, Is.EqualTo(16));
        });
    }
    
    [Test]
    public void TestExitLengthBarrier()
    {
        var packet = new BinaryPacket(ExitLengthBarrierBuffer.AsSpan());
        
        int value = packet.Read<int>();
        
        Assert.Multiple(() =>
        {
            Assert.That(value, Is.EqualTo(0x12345678));
        });
    }

    [Test]
    public void TestReadSpanDirectly()
    {
        var packet = new BinaryPacket(ExitLengthBarrierBuffer.AsSpan());
        Span<byte> span = stackalloc byte[4];
        packet.ReadBytes(span); 
        
        var result = span.ToArray();
        
        Assert.Multiple(() =>
        {
            Assert.That(result[0], Is.EqualTo(0x12));
            Assert.That(result[1], Is.EqualTo(0x34));
            Assert.That(result[2], Is.EqualTo(0x56));
            Assert.That(result[3], Is.EqualTo(0x78));
        });
    }
}