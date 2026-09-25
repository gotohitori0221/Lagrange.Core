using System;
using System.Text;
using Net.Codecrete.QrCodeGenerator;

namespace Lagrange.Milky.Extensions;

public static class QrCodeExtension
{
    public static string ToAscii(this QrCode qrCode, bool compatible)
    {
        
        
        
        var (bottomHalfBlock, topHalfBlock, emptyBlock, fullBlock) = compatible
            ? (".", "^", " ", "@")
            : ("▄", "▀", " ", "█");

        StringBuilder result = new();
        for (int y = 0; y < qrCode.Size + 2; y += 2)
        {
            for (int x = 0; x < qrCode.Size + 2; x++)
            {
                bool foregroundBlack = qrCode.GetModule(x - 1, y - 1);
                bool backgroundBlack = qrCode.GetModule(x - 1, y) || y > qrCode.Size;

                result.Append(
                    foregroundBlack && !backgroundBlack ? bottomHalfBlock :
                    !foregroundBlack && backgroundBlack ? topHalfBlock :
                    foregroundBlack && backgroundBlack  ? emptyBlock :
                    fullBlock);
            }
            result.Append(Environment.NewLine);
        }
        return result.ToString();
    }
}