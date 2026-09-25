using System.Security.Cryptography;
using Lagrange.Core.Utility.Cryptography;

namespace Lagrange.Core.Test.Cryptography;

public class TeaTest
{
    private byte[] _data;
    
    private byte[] _key;
    
    [SetUp]
    public void Setup()
    {
        _data = new byte[1024 * 1024 * 10]; 
        _key = new byte[16];
        
        RandomNumberGenerator.Fill(_key.AsSpan());
    }
    
    [Test]
    public void Test()
    {
        var encrypted = TeaProvider.Encrypt(_data, _key);
        var decrypted = TeaProvider.Decrypt(encrypted, _key);
        Assert.Multiple(() =>
        {
            Assert.That(encrypted, Is.Not.Null);
            Assert.That(decrypted, Is.Not.Null);
        });
        Assert.That(decrypted, Is.EqualTo(_data));
        
        Assert.Pass();
    }
}