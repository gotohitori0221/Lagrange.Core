using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Lagrange.Codec.Entities;

[DebuggerVisualizer("{ToString(),nq}")]
[StructLayout(LayoutKind.Sequential)]
public struct VideoInfo
{
    public int Width;
    public int Height;
    public long Duration;
    
    public override string ToString() => $"Width: {Width}, Height: {Height}, Duration: {Duration}";
}