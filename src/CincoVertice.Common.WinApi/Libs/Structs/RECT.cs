using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs.Structs;

/// <summary>
///     The RECT structure defines the coordinates of the upper-left and lower-right corners of a rectangle.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    /// <summary>The x-coordinate of the upper-left corner of the rectangle.</summary>
    public int Left;

    /// <summary>The y-coordinate of the upper-left corner of the rectangle.</summary>
    public int Top;

    /// <summary>The x-coordinate of the lower-right corner of the rectangle.</summary>
    public int Right;

    /// <summary>The y-coordinate of the lower-right corner of the rectangle.</summary>
    public int Bottom;
}
