using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;
using CincoVertice.Common.WinApi.Libs;
using CincoVertice.Common.WinApi.Libs.Enums;
using CincoVertice.Common.WinApi.Libs.Structs;

namespace CincoVertice.Common.WinApi.Helpers;

public static class WindowHelper
{
    /// <summary>
    ///     Brings the window handle to the foreground and activates the window.
    /// </summary>
    /// <param name="windowHandle">
    ///     A handle to the window that should be activated and brought to the foreground.
    /// </param>
    /// <returns>
    ///     If the window was brought to the foreground, the return value is nonzero. Otherwise, the return value is
    ///     zero.
    /// </returns>
    public static int ActivateWindow(IntPtr windowHandle)
    {
        if (WinUser.IsIconic(windowHandle))
        {
            WinUser.ShowWindow(windowHandle, SW.SW_RESTORE);
        }

        return User32.SetForegroundWindow(windowHandle);
    }

    /// <summary>
    ///     Brings a window of this application to the foreground and gives it the keyboard focus, even when another
    ///     application is active, e.g. a dialog opened from a global hotkey. Plain SetForegroundWindow is refused
    ///     then, so the calling thread briefly shares the input state of the foreground window's thread.
    /// </summary>
    /// <param name="windowHandle">A window created by the calling thread.</param>
    /// <returns>True when the window is the foreground window afterwards.</returns>
    public static bool ForceForeground(nint windowHandle)
    {
        nint foreground = WinUser.GetForegroundWindow();

        if (foreground == windowHandle)
        {
            return true;
        }

        uint currentThread = Kernel32.GetCurrentThreadId();
        uint foregroundThread = foreground == 0 ? 0 : WinUser.GetWindowThreadProcessId(foreground, out _);
        bool attached = foregroundThread != 0
            && foregroundThread != currentThread
            && WinUser.AttachThreadInput(currentThread, foregroundThread, true);

        try
        {
            WinUser.BringWindowToTop(windowHandle);
            User32.SetForegroundWindow(windowHandle);
        }
        finally
        {
            if (attached)
            {
                WinUser.AttachThreadInput(currentThread, foregroundThread, false);
            }
        }

        return WinUser.GetForegroundWindow() == windowHandle;
    }

    /// <summary>
    ///     The title of the foreground window, the one the user works in.
    /// </summary>
    /// <returns>The title; empty when no window is in the foreground or it has no title.</returns>
    public static string GetForegroundWindowTitle()
    {
        nint foreground = WinUser.GetForegroundWindow();
        int length = foreground == 0 ? 0 : WinUser.GetWindowTextLength(foreground);

        if (length == 0)
        {
            return string.Empty;
        }

        var title = new StringBuilder(length + 1);
        WinUser.GetWindowText(foreground, title, title.Capacity);

        return title.ToString();
    }

    /// <summary>
    ///     A top-level window as the user sees it: without the invisible borders of a few pixels that Windows 10 and
    ///     later put around it for resizing. In the DPI awareness of the calling thread.
    /// </summary>
    public static Rectangle GetVisibleBounds(nint window)
    {
        int result = Dwmapi.DwmGetWindowAttribute(
            window,
            (int)DwmWindowAttribute.DWMWA_EXTENDED_FRAME_BOUNDS,
            out RECT frame,
            Marshal.SizeOf<RECT>());

        if (result != 0)
        {
            User32.GetWindowRect(window, out frame);
        }

        return Rectangle.FromLTRB(frame.Left, frame.Top, frame.Right, frame.Bottom);
    }

    /// <summary>
    ///     Moves and sizes a top-level window so that what the user sees of it fills the bounds, without moving it in
    ///     the Z order or activating it. A maximized or minimized window is restored first. In the DPI awareness of the
    ///     calling thread: physical pixels inside <see cref="DpiAwarenessScope.PerMonitorV2"/>.
    /// </summary>
    /// <exception cref="Win32Exception">
    ///     When Windows refuses, e.g. with ERROR_ACCESS_DENIED (5) for a window of a program that runs as administrator
    ///     when this one doesn't.
    /// </exception>
    public static void MoveVisibleBounds(nint window, Rectangle bounds)
    {
        if (WinUser.IsIconic(window) || User32.IsZoomed(window))
        {
            WinUser.ShowWindow(window, SW.SW_RESTORE);
        }

        // Twice: on a monitor with other scaling, the program may change its own size to that scaling after the
        // first move, and the invisible borders with it
        SetVisibleBounds(window, bounds);
        SetVisibleBounds(window, bounds);
    }

    private static void SetVisibleBounds(nint window, Rectangle bounds)
    {
        User32.GetWindowRect(window, out RECT outer);
        Rectangle visible = GetVisibleBounds(window);

        int left = visible.Left - outer.Left;
        int top = visible.Top - outer.Top;
        int right = outer.Right - visible.Right;
        int bottom = outer.Bottom - visible.Bottom;

        bool moved = User32.SetWindowPos(
            window,
            0,
            bounds.Left - left,
            bounds.Top - top,
            bounds.Width + left + right,
            bounds.Height + top + bottom,
            User32.SetWindowPosFlags.SWP_NOZORDER
            | User32.SetWindowPosFlags.SWP_NOACTIVATE
            | User32.SetWindowPosFlags.SWP_NOOWNERZORDER);

        if (!moved)
        {
            throw new Win32Exception(Marshal.GetLastPInvokeError());
        }
    }

    /// <summary>
    ///     Whether the window is one the user moves around: a visible top-level window with a title bar or a border to
    ///     resize it with, and not the desktop or a tool window such as a floating toolbar.
    /// </summary>
    public static bool IsMovableWindow(nint window)
    {
        if (window == 0
            || window == User32.GetShellWindow()
            || User32.GetAncestor(window, User32.GA_ROOT) != window
            || !User32.IsWindowVisible(window))
        {
            return false;
        }

        long style = User32.GetWindowLongPtr(window, User32.GWL_STYLE);
        long extendedStyle = User32.GetWindowLongPtr(window, User32.GWL_EXSTYLE);

        return (extendedStyle & (long)WS.WS_EX_TOOLWINDOW) == 0
            && ((style & User32.WS_CAPTION) == User32.WS_CAPTION || (style & User32.WS_THICKFRAME) != 0);
    }
}
