using System.Runtime.InteropServices;
using CincoVertice.Common.WinApi.Libs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     The monitors' work areas: each monitor without the taskbar. In the DPI awareness of the calling thread: physical
///     pixels on every monitor inside <see cref="DpiAwarenessScope.PerMonitorV2"/>.
/// </summary>
public static class MonitorHelper
{
    /// <returns>The work area of every monitor, in the order Windows gives them.</returns>
    public static IReadOnlyList<Rectangle> GetWorkAreas()
    {
        List<Rectangle> areas = [];

        User32.MonitorEnumProc proc = (monitor, _, _, _) =>
        {
            if (TryGetWorkArea(monitor, out Rectangle area))
            {
                areas.Add(area);
            }

            return true;
        };

        User32.EnumDisplayMonitors(0, 0, Marshal.GetFunctionPointerForDelegate(proc), 0);
        GC.KeepAlive(proc);

        return areas;
    }

    /// <returns>The work area of the monitor the window is on, or the nearest one.</returns>
    public static Rectangle GetWorkArea(nint window)
    {
        nint monitor = User32.MonitorFromWindow(window, User32.MONITOR_DEFAULTTONEAREST);

        return TryGetWorkArea(monitor, out Rectangle area) ? area : Rectangle.Empty;
    }

    private static bool TryGetWorkArea(nint monitor, out Rectangle area)
    {
        var info = new User32.MONITORINFO { cbSize = Marshal.SizeOf<User32.MONITORINFO>() };

        if (monitor == 0 || !User32.GetMonitorInfo(monitor, ref info))
        {
            area = Rectangle.Empty;

            return false;
        }

        area = Rectangle.FromLTRB(info.rcWork.Left, info.rcWork.Top, info.rcWork.Right, info.rcWork.Bottom);

        return true;
    }
}
