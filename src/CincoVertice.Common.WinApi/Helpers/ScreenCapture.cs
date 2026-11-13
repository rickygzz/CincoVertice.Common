using System.ComponentModel;
using System.Drawing.Imaging;
using CincoVertice.Common.WinApi.Libs;
using CincoVertice.Common.WinApi.Libs.Enums;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Takes screenshots. With monitors at different scaling, call it inside <see cref="DpiAwarenessScope"/>, so the
///     bounds and the pixels are physical ones.
/// </summary>
public static class ScreenCapture
{
    /// <summary>The rectangle around every monitor, in screen coordinates; its left or top can be negative.</summary>
    public static Rectangle VirtualScreenBounds => SystemInformation.VirtualScreen;

    /// <summary>
    ///     Copies what is on the screen in the bounds, including layered windows such as tooltips.
    /// </summary>
    /// <param name="bounds">The area to copy, in screen coordinates, e.g. <see cref="VirtualScreenBounds"/>.</param>
    /// <returns>A new bitmap of the bounds' size; the caller disposes it.</returns>
    public static Bitmap Capture(Rectangle bounds)
    {
        var bitmap = new Bitmap(bounds.Width, bounds.Height, PixelFormat.Format32bppArgb);

        try
        {
            using var graphics = Graphics.FromImage(bitmap);

            // Not Graphics.CopyFromScreen: it refuses SourceCopy | CaptureBlt, and without CaptureBlt
            // layered windows are missing
            nint target = graphics.GetHdc();
            nint screen = User32.GetWindowDC(0);

            try
            {
                if (GDI32.BitBlt(
                    target,
                    0,
                    0,
                    bounds.Width,
                    bounds.Height,
                    screen,
                    bounds.X,
                    bounds.Y,
                    RasterOperationCode.SRCCOPY | RasterOperationCode.CAPTUREBLT) == 0)
                {
                    throw new Win32Exception();
                }
            }
            finally
            {
                User32.ReleaseDC(0, screen);
                graphics.ReleaseHdc(target);
            }

            return bitmap;
        }
        catch
        {
            bitmap.Dispose();

            throw;
        }
    }
}
