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
}
