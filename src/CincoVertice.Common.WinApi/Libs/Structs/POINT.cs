using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs.Structs;

/// <summary>
///     The POINT structure defines the x- and y- coordinates of a point.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    /// <summary>The x-coordinate of the point.</summary>
    public int X;

    /// <summary>The y-coordinate of the point.</summary>
    public int Y;
}
