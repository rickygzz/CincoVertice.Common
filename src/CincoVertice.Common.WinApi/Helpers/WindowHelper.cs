using System.Text;
using CincoVertice.Common.WinApi.Libs;
using CincoVertice.Common.WinApi.Libs.Enums;

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
}
