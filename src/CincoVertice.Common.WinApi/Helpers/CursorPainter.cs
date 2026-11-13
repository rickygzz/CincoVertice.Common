using System.Runtime.InteropServices;
using CincoVertice.Common.WinApi.Libs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Draws the mouse pointer as it is on the screen now, into an image of the screen: a copy of the screen doesn't
///     have it, since Windows draws the pointer over the screen apart. In the DPI awareness of the calling thread:
///     physical pixels inside <see cref="DpiAwarenessScope.PerMonitorV2"/>, as the copy of the screen must be.
/// </summary>
public static class CursorPainter
{
    /// <param name="graphics">The image of the screen.</param>
    /// <param name="origin">Where the image's top left corner is on the screen.</param>
    /// <returns>True when the pointer was drawn; false when it's hidden, e.g. while typing in some programs.</returns>
    public static bool Draw(Graphics graphics, Point origin)
    {
        var cursor = new User32.CURSORINFO { cbSize = Marshal.SizeOf<User32.CURSORINFO>() };

        if (!User32.GetCursorInfo(ref cursor)
            || (cursor.flags & User32.CURSOR_SHOWING) == 0
            || cursor.hCursor == 0)
        {
            return false;
        }

        int hotX = 0;
        int hotY = 0;

        // The point that points is at the pointer's position, not the image's corner
        if (User32.GetIconInfo(cursor.hCursor, out User32.ICONINFO icon))
        {
            hotX = icon.xHotspot;
            hotY = icon.yHotspot;
            DeleteBitmap(icon.hbmMask);
            DeleteBitmap(icon.hbmColor);
        }

        nint hdc = graphics.GetHdc();

        try
        {
            return User32.DrawIconEx(
                hdc,
                cursor.ptScreenPos.X - origin.X - hotX,
                cursor.ptScreenPos.Y - origin.Y - hotY,
                cursor.hCursor,
                0,
                0,
                0,
                0,
                User32.DI_NORMAL);
        }
        finally
        {
            graphics.ReleaseHdc(hdc);
        }
    }

    private static void DeleteBitmap(nint bitmap)
    {
        if (bitmap != 0)
        {
            GDI32.DeleteObject(bitmap);
        }
    }
}
