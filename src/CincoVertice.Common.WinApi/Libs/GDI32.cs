using CincoVertice.Common.WinApi.Libs.Enums;
using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;

/// <summary>
/// GDI32
/// </summary>
public class GDI32
{
    /// <summary>
    ///     The BitBlt function performs a bit-block transfer of the color data corresponding to a rectangle of
    ///     pixels from the specified source device context into a destination device context.
    /// </summary>
    /// <param name="hdc">A handle to the destination device context.</param>
    /// <param name="x">
    ///     The x-coordinate, in logical units, of the upper-left corner of the destination rectangle.
    /// </param>
    /// <param name="y">
    ///     The y-coordinate, in logical units, of the upper-left corner of the destination rectangle.
    /// </param>
    /// <param name="cxWidth">The width, in logical units, of the source and destination rectangles.</param>
    /// <param name="cyHeight">The height, in logical units, of the source and the destination rectangles.</param>
    /// <param name="hdcSrc">A handle to the source device context.</param>
    /// <param name="xSrc">
    ///     The x-coordinate, in logical units, of the upper-left corner of the source rectangle.
    /// </param>
    /// <param name="ySrc">
    ///     The y-coordinate, in logical units, of the upper-left corner of the source rectangle.
    /// </param>
    /// <param name="rop">
    ///     A raster-operation code. These codes define how the color data for the source rectangle is to be
    ///     combined with the color data for the destination rectangle to achieve the final color.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.
    ///     <para>
    ///         If the function fails, the return value is zero. To get extended error information, call
    ///         GetLastError.
    ///     </para>
    /// </returns>
    [DllImport("gdi32.dll", SetLastError = true)]
    public static extern int BitBlt(
        nint hdc,
        int x,
        int y,
        int cxWidth,
        int cyHeight,
        nint hdcSrc,
        int xSrc,
        int ySrc,
        RasterOperationCode rop);

    /// <summary>
    ///     The CreateCompatibleBitmap function creates a bitmap compatible with the device that is associated with
    ///     the specified device context.
    /// </summary>
    /// <param name="hDC">A handle to a device context.</param>
    /// <param name="cxWidth">The bitmap width, in pixels.</param>
    /// <param name="cyHeight">The bitmap height, in pixels.</param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the compatible bitmap (DDB).
    ///     <para>If the function fails, the return value is NULL.</para>
    /// </returns>
    [DllImport("gdi32.dll")]
    public static extern nint CreateCompatibleBitmap(nint hDC, int cxWidth, int cyHeight);

    /// <summary>
    /// The CreateCompatibleDC function creates a memory device context (DC) compatible with the specified device.
    /// </summary>
    /// <param name="hDC">
    ///     A handle to an existing DC. If this handle is NULL, the function creates a memory DC compatible with the
    ///     application's current screen.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is the handle to a memory DC.
    ///     <para>If the function fails, the return value is NULL.</para>
    /// </returns>
    [DllImport("gdi32.dll")]
    public static extern nint CreateDC(string sDriver, string sDevice, string sPort, nint pdm);

    /// <summary>
    /// The CreateCompatibleDC function creates a memory device context (DC) compatible with the specified device.
    /// </summary>
    /// <param name="hDC">
    ///     A handle to an existing DC. If this handle is NULL, the function creates a memory DC compatible with the
    ///     application's current screen.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is the handle to a memory DC.
    ///     <para>If the function fails, the return value is NULL.</para>
    /// </returns>
    [DllImport("gdi32.dll")]
    public static extern nint CreateCompatibleDC(nint hDC);

    /// <summary>
    ///     The DeleteDC function deletes the specified device context (DC).
    /// </summary>
    /// <param name="hDC">A handle to the device context.</param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.
    ///     <para>If the function fails, the return value is zero.</para>
    /// </returns>
    [DllImport("gdi32.dll")]
    public static extern int DeleteDC(nint hDC);

    /// <summary>
    ///     The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all
    ///     system resources associated with the object. After the object is deleted, the specified handle is no
    ///     longer valid.
    /// </summary>
    /// <param name="hObject">A handle to a logical pen, brush, font, bitmap, region, or palette.</param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.
    ///     <para>
    ///         If the specified handle is not valid or is currently selected into a DC, the return value is zero.
    ///     </para>
    /// </returns>
    [DllImport("gdi32.dll")]
    public static extern int DeleteObject(nint hObject);

    /// <summary>
    ///     The SelectObject function selects an object into the specified device context (DC). The new object
    ///     replaces the previous object of the same type.
    /// </summary>
    /// <param name="hDC">A handle to the DC.</param>
    /// <param name="hObject">
    ///     A handle to the object to be selected. The specified object must have been created by using one of the
    ///     following functions.
    /// </param>
    /// <returns>
    ///     If an error occurs and the selected object is not a region, the return value is NULL. Otherwise, it is
    ///     HGDI_ERROR.
    /// </returns>
    [DllImport("gdi32.dll")]
    public static extern nint SelectObject(nint hDC, nint hObject);
}
