using Net.Codecrete.QrCodeGenerator;

namespace Lagrange.Core.Runner;

internal static class QrCodeHelper
{
    
    
    internal static void Output(string text, bool compatibilityMode)
    {
        var segments = QrSegment.MakeSegments(text);
        var qrCode = QrCode.EncodeSegments(segments, QrCode.Ecc.Low);

        var (bottomHalfBlock, topHalfBlock, emptyBlock, fullBlock) = compatibilityMode ? (".", "^", " ", "@") : ("▄", "▀", " ", "█");

        for (var y = 0; y < qrCode.Size + 2; y += 2)
        {
            for (var x = 0; x < qrCode.Size + 2; ++x)
            {
                var foregroundBlack = qrCode.GetModule(x - 1, y - 1);
                var backgroundBlack = qrCode.GetModule(x - 1, y) || y > qrCode.Size;

                if (foregroundBlack && !backgroundBlack)
                {
                    Console.Write(bottomHalfBlock);
                }
                else if (!foregroundBlack && backgroundBlack)
                {
                    Console.Write(topHalfBlock);
                }
                else if (foregroundBlack && backgroundBlack)
                {
                    Console.Write(emptyBlock);

                }
                else if (!foregroundBlack && !backgroundBlack)
                {
                    Console.Write(fullBlock);
                }
            }
            Console.Write("\n");
        }

        if (compatibilityMode)
        {
            Console.WriteLine("Please scan this QR code from a distance with your smart phone.\nScanning may fail if you are too close.");
        }
    }
}