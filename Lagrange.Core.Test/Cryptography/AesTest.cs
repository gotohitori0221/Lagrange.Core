using System.Security.Cryptography;
using Lagrange.Core.Utility.Cryptography;

namespace Lagrange.Core.Test.Cryptography;

public class AesTest
{
    private byte[] _data;
    
    private byte[] _key;

    [SetUp]
    public void Setup()
    {
        _data = new byte[0x10000];
        _key = new byte[0x20];
        
        RandomNumberGenerator.Fill(_data);
        RandomNumberGenerator.Fill(_key);
    }
    
    [Test]
    public void TestEncrypt()
    {
        var cipher = AesGcmProvider.Encrypt(_data, _key);
        var plain = AesGcmProvider.Decrypt(cipher, _key);
        
        Assert.That(plain, Is.EqualTo(_data));
    }
}