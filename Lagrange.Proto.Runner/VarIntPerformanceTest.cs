using System.Buffers;
using System.Diagnostics;
using Lagrange.Proto.Primitives;

namespace Lagrange.Proto.Runner;

public static class VarIntPerformanceTest
{
    public static void Run()
    {
        const int count = 1_000_000;
        var random = new Random(42);

        
        var values1 = new uint[count];
        var values2 = new uint[count];

        for (int i = 0; i < count; i++)
        {
            
            values1[i] = (uint)(i % 5) switch
            {
                0 => (uint)random.Next(0, 128),              
                1 => (uint)random.Next(128, 16384),          
                2 => (uint)random.Next(16384, 2097152),      
                3 => (uint)random.Next(2097152, 268435456),  
                _ => (uint)random.Next(268435456, int.MaxValue) 
            };

            values2[i] = (uint)((i + 2) % 5) switch
            {
                0 => (uint)random.Next(0, 128),
                1 => (uint)random.Next(128, 16384),
                2 => (uint)random.Next(16384, 2097152),
                3 => (uint)random.Next(2097152, 268435456),
                _ => (uint)random.Next(268435456, int.MaxValue)
            };
        }

        
        Console.WriteLine("=== Validation Test ===");
        Console.WriteLine("Verifying that Sequential and SIMD encoding produce identical output...");

        bool validationPassed = ValidateEncodingEquality(values1, values2, Math.Min(10000, count));

        if (validationPassed)
        {
            Console.WriteLine("✓ Validation PASSED: Both methods produce identical output");
        }
        else
        {
            Console.WriteLine("✗ Validation FAILED: Methods produce different output!");
            return; 
        }

        
        Console.WriteLine("\nWarming up...");
        RunSequentialTest(values1, values2, 1000);
        RunSimdTest(values1, values2, 1000);

        Console.WriteLine("\n=== VarInt Performance Test ===");
        Console.WriteLine($"Encoding {count:N0} pairs of uint32 values ({count * 2:N0} total values)\n");

        
        Console.WriteLine("Test 1: Sequential EncodeVarInt (2x per iteration)");
        var sequentialTime = RunSequentialTest(values1, values2, count);
        Console.WriteLine($"  Time: {sequentialTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"  Throughput: {count * 2 / sequentialTime.TotalSeconds:F0} values/sec");
        Console.WriteLine($"  Per value: {sequentialTime.TotalNanoseconds / (count * 2):F1} ns");

        
        Console.WriteLine("\nTest 2: EncodeTwo32VarIntUnsafe (SIMD)");
        var simdTime = RunSimdTest(values1, values2, count);
        Console.WriteLine($"  Time: {simdTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"  Throughput: {count * 2 / simdTime.TotalSeconds:F0} values/sec");
        Console.WriteLine($"  Per value: {simdTime.TotalNanoseconds / (count * 2):F1} ns");

        
        var speedup = sequentialTime.TotalMilliseconds / simdTime.TotalMilliseconds;
        Console.WriteLine($"\nSpeedup: {speedup:F2}x");
        Console.WriteLine($"Time saved: {(sequentialTime - simdTime).TotalMilliseconds:F2} ms ({(1 - 1/speedup) * 100:F1}%)");

        
        Console.WriteLine("\nTest 3: Mixed Strategy (small sequential, large SIMD)");
        var mixedTime = RunMixedTest(values1, values2, count);
        Console.WriteLine($"  Time: {mixedTime.TotalMilliseconds:F2} ms");
        Console.WriteLine($"  Throughput: {count * 2 / mixedTime.TotalSeconds:F0} values/sec");
        Console.WriteLine($"  Per value: {mixedTime.TotalNanoseconds / (count * 2):F1} ns");
        Console.WriteLine($"  Speedup vs Sequential: {sequentialTime.TotalMilliseconds / mixedTime.TotalMilliseconds:F2}x");
    }

    private static TimeSpan RunSequentialTest(uint[] values1, uint[] values2, int count)
    {
        var buffer = new ArrayBufferWriter<byte>(count * 10);
        var writer = new ProtoWriter(buffer);

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < count; i++)
        {
            writer.EncodeVarInt(values1[i]);
            writer.EncodeVarInt(values2[i]);
        }
        writer.Flush();

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    private static TimeSpan RunSimdTest(uint[] values1, uint[] values2, int count)
    {
        var buffer = new ArrayBufferWriter<byte>(count * 10);
        var writer = new ProtoWriter(buffer);

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < count; i++)
        {
            writer.EncodeTwo32VarIntUnsafe(values1[i], values2[i]);
        }
        writer.Flush();

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    private static TimeSpan RunMixedTest(uint[] values1, uint[] values2, int count)
    {
        var buffer = new ArrayBufferWriter<byte>(count * 10);
        var writer = new ProtoWriter(buffer);

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < count; i++)
        {
            uint v1 = values1[i];
            uint v2 = values2[i];

            
            if (v1 < 128 && v2 < 128)
            {
                writer.EncodeVarInt(v1);
                writer.EncodeVarInt(v2);
            }
            else
            {
                writer.EncodeTwo32VarIntUnsafe(v1, v2);
            }
        }
        writer.Flush();

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    private static bool ValidateEncodingEquality(uint[] values1, uint[] values2, int count)
    {
        
        var sequentialBuffer = new ArrayBufferWriter<byte>(count * 10);
        var sequentialWriter = new ProtoWriter(sequentialBuffer);

        for (int i = 0; i < count; i++)
        {
            sequentialWriter.EncodeVarInt(values1[i]);
            sequentialWriter.EncodeVarInt(values2[i]);
        }
        sequentialWriter.Flush();

        var sequentialBytes = sequentialBuffer.WrittenMemory.ToArray();

        
        var simdBuffer = new ArrayBufferWriter<byte>(count * 10);
        var simdWriter = new ProtoWriter(simdBuffer);

        for (int i = 0; i < count; i++)
        {
            simdWriter.EncodeTwo32VarIntUnsafe(values1[i], values2[i]);
        }
        simdWriter.Flush();

        var simdBytes = simdBuffer.WrittenMemory.ToArray();

        
        if (sequentialBytes.Length != simdBytes.Length)
        {
            Console.WriteLine($"  Length mismatch: Sequential={sequentialBytes.Length}, SIMD={simdBytes.Length}");
            return false;
        }

        for (int i = 0; i < sequentialBytes.Length; i++)
        {
            if (sequentialBytes[i] != simdBytes[i])
            {
                Console.WriteLine($"  First difference at byte {i} of {sequentialBytes.Length}:");
                Console.WriteLine($"    Sequential[{i}]: 0x{sequentialBytes[i]:X2}");
                Console.WriteLine($"    SIMD[{i}]: 0x{simdBytes[i]:X2}");

                
                int start = Math.Max(0, i - 5);
                int end = Math.Min(sequentialBytes.Length, i + 6);
                Console.WriteLine($"    Context (bytes {start}-{end - 1}):");
                Console.WriteLine($"      Sequential: {BitConverter.ToString(sequentialBytes, start, end - start)}");
                Console.WriteLine($"      SIMD:       {BitConverter.ToString(simdBytes, start, end - start)}");

                
                var reader = new ProtoReader(sequentialBytes.AsSpan());
                int pairIndex = 0;
                int currentPos = 0;

                try
                {
                    while (!reader.IsCompleted && currentPos < i)
                    {
                        var v1 = reader.DecodeVarInt<uint>();
                        var v2 = reader.DecodeVarInt<uint>();

                        
                        currentPos += GetVarIntSize(v1) + GetVarIntSize(v2);

                        if (currentPos > i)
                        {
                            Console.WriteLine($"    Likely pair index: {pairIndex} (values: {values1[pairIndex]}, {values2[pairIndex]})");
                            break;
                        }
                        pairIndex++;
                    }
                }
                catch
                {
                    
                }

                return false;
            }
        }

        Console.WriteLine($"  Validated {count} pairs ({count * 2} values, {sequentialBytes.Length} bytes)");
        return true;
    }

    private static int GetVarIntSize(uint value)
    {
        if (value < 128) return 1;
        if (value < 16384) return 2;
        if (value < 2097152) return 3;
        if (value < 268435456) return 4;
        return 5;
    }
}