using CincoVertice.Common.WinApi.Libs.Enums;
using CincoVertice.Common.WinApi.Libs.Structs;
using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;

public static class Dwmapi
{
    /// <summary>
    ///     Retrieves the current value of a specified Desktop Window Manager (DWM) attribute applied to a window.
    ///     For programming guidance, and code examples, see Controlling non-client region rendering.
    /// </summary>
    /// <param name="hwnd">The handle to the window from which the attribute value is to be retrieved.</param>
    /// <param name="dwAttribute">
    ///     A flag describing which value to retrieve, specified as a value of the DWMWINDOWATTRIBUTE enumeration.
    ///     This parameter specifies which attribute to retrieve, and the pvAttribute parameter points to an object
    ///     into which the attribute value is retrieved.
    /// </param>
    /// <param name="pvAttribute">
    ///     A pointer to a value which, when this function returns successfully, receives the current value of the
    ///     attribute. The type of the retrieved value depends on the value of the dwAttribute parameter. The
    ///     DWMWINDOWATTRIBUTE enumeration topic indicates, in the row for each flag, what type of value you should
    ///     pass a pointer to in the pvAttribute parameter.
    /// </param>
    /// <param name="cbAttribute">
    ///     The size, in bytes, of the attribute value being received via the pvAttribute parameter. The type of the
    ///     retrieved value, and therefore its size in bytes, depends on the value of the dwAttribute parameter.
    /// </param>
    /// <returns></returns>
    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(
        nint hwnd,
        int dwAttribute,
        out RECT pvAttribute,
        int cbAttribute);

    /// <summary>
    /// GetWindowRectangle without adornments (like shadow).
    /// </summary>
    /// <param name="hWnd">hWnd.</param>
    /// <returns>RECT.</returns>
    public static RECT GetWindowRectangle(nint hWnd)
    {
        int size = Marshal.SizeOf<RECT>();
        DwmGetWindowAttribute(hWnd, (int)DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS, out RECT rect, size);

        return rect;
    }
}
