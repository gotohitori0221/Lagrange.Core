using System.Security.Cryptography;
using Lagrange.Core.Utility.Cryptography;

namespace Lagrange.Core.Test.Cryptography;

[TestFixture]
[Parallelizable]
public class EcdhTest
{
    private EcdhProvider _alice;
    private EcdhProvider _bob;
    private EcdhProvider _customSecret;

    [SetUp]
    public void Setup()
    {
        _alice = new EcdhProvider(EllipticCurve.Prime256V1);
        _bob = new EcdhProvider(EllipticCurve.Prime256V1);

        var secret = _alice.PackSecret();
        _customSecret = new EcdhProvider(EllipticCurve.Prime256V1, secret); 
    }

    #region Basic ECDH Tests

    [Test]
    public void Test_BasicKeyExchange_CompressedKeys()
    {
        var alicePubCompressed = _alice.PackPublic(true);
        var bobPubCompressed = _bob.PackPublic(true);

        var aliceSharedPacked = _alice.KeyExchange(bobPubCompressed, true);
        var bobSharedPacked = _bob.KeyExchange(alicePubCompressed, true);

        Assert.Multiple(() =>
        {
            Assert.That(aliceSharedPacked, Is.EqualTo(bobSharedPacked), "Shared secrets should match");
            Assert.That(aliceSharedPacked, Has.Length.EqualTo(16), "MD5 hash should be 16 bytes");
        });
    }

    [Test]
    public void Test_BasicKeyExchange_UncompressedKeys()
    {
        var alicePub = _alice.PackPublic(false);
        var bobPub = _bob.PackPublic(false);

        var aliceShared = _alice.KeyExchange(bobPub, true);
        var bobShared = _bob.KeyExchange(alicePub, true);

        Assert.Multiple(() =>
        {
            Assert.That(aliceShared, Is.EqualTo(bobShared), "Shared secrets should match");
            Assert.That(aliceShared, Has.Length.EqualTo(16), "MD5 hash should be 16 bytes");
        });
    }

    [Test]
    public void TestCustomSecret()
    {
        var customPub = _customSecret.PackPublic(false);
        var alicePub = _alice.PackPublic(false);

        Assert.That(customPub, Is.EqualTo(alicePub), "Same secret should generate same public key");
    }

    [Test]
    public void Test_SecretUniqueness()
    {
        var aliceSecret = _alice.PackSecret();
        var bobSecret = _bob.PackSecret();

        Assert.Multiple(() =>
        {
            Assert.That(aliceSecret, Is.Not.EqualTo(bobSecret), "Secrets should be unique");
            Assert.That(aliceSecret, Has.Length.EqualTo(aliceSecret[3] + 4), "Secret length should match metadata");
            Assert.That(bobSecret, Has.Length.EqualTo(bobSecret[3] + 4), "Secret length should match metadata");
        });
    }

    #endregion

    #region Fixed-Size Output Tests (Critical for the bug fix)

    [Test]
    public void Test_SharedSecret_FixedSize_Prime256V1_NoHash()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var bob = new EcdhProvider(EllipticCurve.Prime256V1);

        var bobPub = bob.PackPublic(false);
        var sharedSecret = alice.KeyExchange(bobPub, false);

        Assert.That(sharedSecret, Has.Length.EqualTo(32),
            "Prime256V1 shared secret without hash should be exactly 32 bytes");
    }

    [Test]
    public void Test_SharedSecret_FixedSize_Prime256V1_WithHash()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var bob = new EcdhProvider(EllipticCurve.Prime256V1);

        var bobPub = bob.PackPublic(false);
        var sharedSecret = alice.KeyExchange(bobPub, true);

        Assert.That(sharedSecret, Has.Length.EqualTo(16),
            "Prime256V1 shared secret with MD5 hash should be exactly 16 bytes");
    }

    [Test]
    public void Test_SharedSecret_FixedSize_Secp192K1_NoHash()
    {
        var alice = new EcdhProvider(EllipticCurve.Secp192K1);
        var bob = new EcdhProvider(EllipticCurve.Secp192K1);

        var bobPub = bob.PackPublic(false);
        var sharedSecret = alice.KeyExchange(bobPub, false);

        Assert.That(sharedSecret, Has.Length.EqualTo(24),
            "Secp192K1 shared secret without hash should be exactly 24 bytes");
    }

    [Test]
    public void Test_SharedSecret_FixedSize_Secp192K1_WithHash()
    {
        var alice = new EcdhProvider(EllipticCurve.Secp192K1);
        var bob = new EcdhProvider(EllipticCurve.Secp192K1);

        var bobPub = bob.PackPublic(false);
        var sharedSecret = alice.KeyExchange(bobPub, true);

        Assert.That(sharedSecret, Has.Length.EqualTo(16),
            "Secp192K1 shared secret with MD5 hash should be exactly 16 bytes");
    }

    [Test]
    public void Test_SharedSecret_ConsistentSize_MultipleExchanges()
    {
        
        var sizes = new HashSet<int>();

        for (int i = 0; i < 10; i++)
        {
            var alice = new EcdhProvider(EllipticCurve.Prime256V1);
            var bob = new EcdhProvider(EllipticCurve.Prime256V1);

            var bobPub = bob.PackPublic(false);
            var sharedSecret = alice.KeyExchange(bobPub, false);
            sizes.Add(sharedSecret.Length);
        }

        Assert.That(sizes, Has.Count.EqualTo(1), "All shared secrets should have the same length");
        Assert.That(sizes.First(), Is.EqualTo(32), "All Prime256V1 shared secrets should be 32 bytes");
    }

    #endregion

    #region Public Key Format Tests

    [Test]
    public void Test_CompressedPublicKey_Prime256V1_Format()
    {
        var provider = new EcdhProvider(EllipticCurve.Prime256V1);
        var compressed = provider.PackPublic(true);

        Assert.Multiple(() =>
        {
            Assert.That(compressed, Has.Length.EqualTo(33), "Compressed Prime256V1 key should be 33 bytes");
            Assert.That(compressed[0], Is.EqualTo(0x02).Or.EqualTo(0x03), "First byte should be 0x02 or 0x03");
        });
    }

    [Test]
    public void Test_UncompressedPublicKey_Prime256V1_Format()
    {
        var provider = new EcdhProvider(EllipticCurve.Prime256V1);
        var uncompressed = provider.PackPublic(false);

        Assert.Multiple(() =>
        {
            Assert.That(uncompressed, Has.Length.EqualTo(65), "Uncompressed Prime256V1 key should be 65 bytes");
            Assert.That(uncompressed[0], Is.EqualTo(0x04), "First byte should be 0x04");
        });
    }

    [Test]
    public void Test_CompressedPublicKey_Secp192K1_Format()
    {
        var provider = new EcdhProvider(EllipticCurve.Secp192K1);
        var compressed = provider.PackPublic(true);

        Assert.Multiple(() =>
        {
            Assert.That(compressed, Has.Length.EqualTo(25), "Compressed Secp192K1 key should be 25 bytes");
            Assert.That(compressed[0], Is.EqualTo(0x02).Or.EqualTo(0x03), "First byte should be 0x02 or 0x03");
        });
    }

    [Test]
    public void Test_UncompressedPublicKey_Secp192K1_Format()
    {
        var provider = new EcdhProvider(EllipticCurve.Secp192K1);
        var uncompressed = provider.PackPublic(false);

        Assert.Multiple(() =>
        {
            Assert.That(uncompressed, Has.Length.EqualTo(49), "Uncompressed Secp192K1 key should be 49 bytes");
            Assert.That(uncompressed[0], Is.EqualTo(0x04), "First byte should be 0x04");
        });
    }

    [Test]
    public void Test_PublicKey_Padding_NoLeadingZerosStripped()
    {
        
        
        var provider = new EcdhProvider(EllipticCurve.Prime256V1);

        
        for (int i = 0; i < 20; i++)
        {
            var newProvider = new EcdhProvider(EllipticCurve.Prime256V1);
            var compressed = newProvider.PackPublic(true);
            var uncompressed = newProvider.PackPublic(false);

            Assert.Multiple(() =>
            {
                Assert.That(compressed, Has.Length.EqualTo(33),
                    $"Iteration {i}: Compressed key should always be 33 bytes");
                Assert.That(uncompressed, Has.Length.EqualTo(65),
                    $"Iteration {i}: Uncompressed key should always be 65 bytes");
            });
        }
    }

    #endregion

    #region Interoperability Tests

    [Test]
    public void Test_CompressedAndUncompressed_ProduceSameSharedSecret()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var bob = new EcdhProvider(EllipticCurve.Prime256V1);

        var bobPubCompressed = bob.PackPublic(true);
        var bobPubUncompressed = bob.PackPublic(false);

        var sharedFromCompressed = alice.KeyExchange(bobPubCompressed, false);
        var sharedFromUncompressed = alice.KeyExchange(bobPubUncompressed, false);

        Assert.That(sharedFromCompressed, Is.EqualTo(sharedFromUncompressed),
            "Shared secret should be the same regardless of public key compression");
    }

    [Test]
    public void Test_SymmetricKeyExchange_Compressed()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var bob = new EcdhProvider(EllipticCurve.Prime256V1);

        var alicePub = alice.PackPublic(true);
        var bobPub = bob.PackPublic(true);

        var aliceShared = alice.KeyExchange(bobPub, false);
        var bobShared = bob.KeyExchange(alicePub, false);

        Assert.That(aliceShared, Is.EqualTo(bobShared),
            "Alice and Bob should derive the same shared secret");
    }

    [Test]
    public void Test_SymmetricKeyExchange_Uncompressed()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var bob = new EcdhProvider(EllipticCurve.Prime256V1);

        var alicePub = alice.PackPublic(false);
        var bobPub = bob.PackPublic(false);

        var aliceShared = alice.KeyExchange(bobPub, false);
        var bobShared = bob.KeyExchange(alicePub, false);

        Assert.That(aliceShared, Is.EqualTo(bobShared),
            "Alice and Bob should derive the same shared secret");
    }

    #endregion

    #region Error Handling Tests

    [Test]
    public void Test_InvalidPublicKey_Length()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var invalidKey = new byte[20]; 

        Assert.Throws<Exception>(() => alice.KeyExchange(invalidKey, false),
            "Should throw exception for invalid public key length");
    }

    [Test]
    public void Test_InvalidPublicKey_NotOnCurve()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);

        
        var invalidKey = new byte[65];
        invalidKey[0] = 0x04;
        RandomNumberGenerator.Fill(invalidKey.AsSpan(1));

        Assert.Throws<Exception>(() => alice.KeyExchange(invalidKey, false),
            "Should throw exception when public key is not on the curve");
    }

    [Test]
    public void Test_InvalidSecret_Length()
    {
        var invalidSecret = new byte[10];
        invalidSecret[3] = 20; 
        RandomNumberGenerator.Fill(invalidSecret.AsSpan(4));

        Assert.Throws<Exception>(() => new EcdhProvider(EllipticCurve.Prime256V1, invalidSecret),
            "Should throw exception when secret length doesn't match metadata");
    }

    #endregion

    #region Secret Packing/Unpacking Tests

    [Test]
    public void Test_SecretPacking_RoundTrip()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var packedSecret = alice.PackSecret();

        var bob = new EcdhProvider(EllipticCurve.Prime256V1, packedSecret);

        var alicePub = alice.PackPublic(false);
        var bobPub = bob.PackPublic(false);

        Assert.That(bobPub, Is.EqualTo(alicePub),
            "Same secret should produce same public key after unpacking");
    }

    [Test]
    public void Test_SecretPacking_Format()
    {
        var provider = new EcdhProvider(EllipticCurve.Prime256V1);
        var packed = provider.PackSecret();

        Assert.Multiple(() =>
        {
            Assert.That(packed, Has.Length.GreaterThanOrEqualTo(4), "Packed secret should have at least 4 bytes");
            Assert.That(packed[3], Is.EqualTo(packed.Length - 4), "Length metadata should match actual data length");
        });
    }

    #endregion

    #region Both Curves Tests

    [Test]
    public void Test_Prime256V1_FullKeyExchange()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var bob = new EcdhProvider(EllipticCurve.Prime256V1);

        var alicePub = alice.PackPublic(false);
        var bobPub = bob.PackPublic(false);

        var aliceShared = alice.KeyExchange(bobPub, false);
        var bobShared = bob.KeyExchange(alicePub, false);

        Assert.Multiple(() =>
        {
            Assert.That(aliceShared, Is.EqualTo(bobShared), "Shared secrets should match");
            Assert.That(aliceShared, Has.Length.EqualTo(32), "Prime256V1 shared secret should be 32 bytes");
        });
    }

    [Test]
    public void Test_Secp192K1_FullKeyExchange()
    {
        var alice = new EcdhProvider(EllipticCurve.Secp192K1);
        var bob = new EcdhProvider(EllipticCurve.Secp192K1);

        var alicePub = alice.PackPublic(false);
        var bobPub = bob.PackPublic(false);

        var aliceShared = alice.KeyExchange(bobPub, false);
        var bobShared = bob.KeyExchange(alicePub, false);

        Assert.Multiple(() =>
        {
            Assert.That(aliceShared, Is.EqualTo(bobShared), "Shared secrets should match");
            Assert.That(aliceShared, Has.Length.EqualTo(24), "Secp192K1 shared secret should be 24 bytes");
        });
    }

    [Test]
    public void Test_DifferentCurves_ProduceDifferentResults()
    {
        var provider1 = new EcdhProvider(EllipticCurve.Prime256V1);
        var provider2 = new EcdhProvider(EllipticCurve.Secp192K1);

        var pub1 = provider1.PackPublic(false);
        var pub2 = provider2.PackPublic(false);

        Assert.Multiple(() =>
        {
            Assert.That(pub1, Has.Length.Not.EqualTo(pub2.Length),
                "Different curves should produce different length public keys");
            Assert.That(pub1, Is.Not.EqualTo(pub2),
                "Different curves should produce different public keys");
        });
    }

    #endregion

    #region Hash vs Non-Hash Tests

    [Test]
    public void Test_HashVsNonHash_DifferentLengths()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var bob = new EcdhProvider(EllipticCurve.Prime256V1);

        var bobPub = bob.PackPublic(false);

        var sharedNoHash = alice.KeyExchange(bobPub, false);
        var sharedWithHash = alice.KeyExchange(bobPub, true);

        Assert.Multiple(() =>
        {
            Assert.That(sharedNoHash, Has.Length.EqualTo(32), "Non-hashed should be 32 bytes");
            Assert.That(sharedWithHash, Has.Length.EqualTo(16), "MD5 hashed should be 16 bytes");
            Assert.That(sharedNoHash, Is.Not.EqualTo(sharedWithHash), "Hashed and non-hashed should be different");
        });
    }

    [Test]
    public void Test_HashedSharedSecret_IsDeterministic()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var bob = new EcdhProvider(EllipticCurve.Prime256V1);

        var bobPub = bob.PackPublic(false);

        var shared1 = alice.KeyExchange(bobPub, true);
        var shared2 = alice.KeyExchange(bobPub, true);

        Assert.That(shared1, Is.EqualTo(shared2),
            "Multiple calls with same keys should produce same hashed result");
    }

    #endregion

    #region Multiple Exchange Tests

    [Test]
    public void Test_MultipleParties_KeyExchange()
    {
        var alice = new EcdhProvider(EllipticCurve.Prime256V1);
        var bob = new EcdhProvider(EllipticCurve.Prime256V1);
        var charlie = new EcdhProvider(EllipticCurve.Prime256V1);

        var alicePub = alice.PackPublic(false);
        var bobPub = bob.PackPublic(false);
        var charliePub = charlie.PackPublic(false);

        var aliceBobShared = alice.KeyExchange(bobPub, false);
        var bobAliceShared = bob.KeyExchange(alicePub, false);

        var aliceCharlieShared = alice.KeyExchange(charliePub, false);
        var charlieAliceShared = charlie.KeyExchange(alicePub, false);

        Assert.Multiple(() =>
        {
            Assert.That(aliceBobShared, Is.EqualTo(bobAliceShared),
                "Alice-Bob shared secret should match");
            Assert.That(aliceCharlieShared, Is.EqualTo(charlieAliceShared),
                "Alice-Charlie shared secret should match");
            Assert.That(aliceBobShared, Is.Not.EqualTo(aliceCharlieShared),
                "Different pairs should have different shared secrets");
        });
    }

    #endregion

    #region Stress Tests

    [Test]
    public void Test_StressTest_100KeyExchanges()
    {
        for (int i = 0; i < 100; i++)
        {
            var alice = new EcdhProvider(EllipticCurve.Prime256V1);
            var bob = new EcdhProvider(EllipticCurve.Prime256V1);

            var alicePub = alice.PackPublic(true);
            var bobPub = bob.PackPublic(true);

            var aliceShared = alice.KeyExchange(bobPub, false);
            var bobShared = bob.KeyExchange(alicePub, false);

            Assert.Multiple(() =>
            {
                Assert.That(aliceShared, Is.EqualTo(bobShared), $"Iteration {i}: Shared secrets should match");
                Assert.That(aliceShared, Has.Length.EqualTo(32), $"Iteration {i}: Should be 32 bytes");
            });
        }
    }

    #endregion
}
