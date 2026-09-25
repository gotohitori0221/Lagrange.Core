using System.Security.Cryptography;
using Lagrange.Core.Utility.Cryptography;

namespace Lagrange.Core.Test.Cryptography;

public class Sha1Test
{
    private byte[] _data;
    
    private Sha1Stream _sha1;
    
    [SetUp]
    public void Setup()
    {
        _data = new byte[1024 * 1024 * 10]; 
        _sha1 = new Sha1Stream();
        
        RandomNumberGenerator.Fill(_data.AsSpan());
    }
    
    [Test]
    public void Test()
    {
        var expected = SHA1.HashData(_data);
        var digest = new byte[Sha1Stream.Sha1DigestSize];
        var intermediate = new byte[Sha1Stream.Sha1BlockSize];
        
        for (int i = 0; i < _data.Length; i += Sha1Stream.Sha1BlockSize)
        {
            _sha1.Hash(intermediate.AsSpan(), false);
            _sha1.Hash(intermediate.AsSpan(), true);
            _sha1.Update(_data.AsSpan(i, Math.Min(Sha1Stream.Sha1BlockSize, _data.Length - i)));
        }
        
        _sha1.Final(digest);
        
        Assert.That(digest, Is.EqualTo(expected));
        
        Assert.Pass();
    }
}