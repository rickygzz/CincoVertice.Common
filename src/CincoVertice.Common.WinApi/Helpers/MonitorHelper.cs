using System.Runtime.InteropServices;
using CincoVertice.Common.WinApi.Libs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     The monitors: where each is, and its work area, the monitor without the taskbar. In the DPI awareness of the
///     calling thread: physical pixels on every monitor inside <see cref="DpiAwarenessScope.PerMonitorV2"/>.
/// </summary>
public static class MonitorHelper
{
    /// <returns>Every monitor, in the order Windows gives them.</returns>
    public static IReadOnlyList<(Rectangle Bounds, Rectangle WorkArea)> GetMonitors()
    {
        List<(Rectangle Bounds, Rectangle WorkArea)> monitors = [];

        User32.MonitorEnumProc proc = (monitor, _, _, _) =>
        {
            if (TryGetMonitor(monitor, out var found))
            {
                monitors.Add(found);
            }

            return true;
        };

        User32.EnumDisplayMonitors(0, 0, Marshal.GetFunctionPointerForDelegate(proc), 0);
        GC.KeepAlive(proc);

        return monitors;
    }

    /// <returns>The work area of every monitor, in the order Windows gives them.</returns>
    public static IReadOnlyList<Rectangle> GetWorkAreas() => [.. GetMonitors().Select(monitor => monitor.WorkArea)];

    /// <returns>The monitor the window is on, or the nearest one; empty rectangles when there is none.</returns>
    public static (Rectangle Bounds, Rectangle WorkArea) GetMonitor(nint window)
    {
        nint monitor = User32.MonitorFromWindow(window, User32.MONITOR_DEFAULTTONEAREST);

        return TryGetMonitor(monitor, out var found) ? found : (Rectangle.Empty, Rectangle.Empty);
    }

    /// <returns>The work area of the monitor the window is on, or the nearest one.</returns>
    public static Rectangle GetWorkArea(nint window) => GetMonitor(window).WorkArea;

    private static bool TryGetMonitor(nint monitor, out (Rectangle Bounds, Rectangle WorkArea) found)
    {
        var info = new User32.MONITORINFO { cbSize = Marshal.SizeOf<User32.MONITORINFO>() };

        if (monitor == 0 || !User32.GetMonitorInfo(monitor, ref info))
        {
            found = (Rectangle.Empty, Rectangle.Empty);

            return false;
        }

        found = (ToRectangle(info.rcMonitor), ToRectangle(info.rcWork));

        return true;
    }

    private static Rectangle ToRectangle(Libs.Structs.RECT rect) =>
        Rectangle.FromLTRB(rect.Left, rect.Top, rect.Right, rect.Bottom);
}
