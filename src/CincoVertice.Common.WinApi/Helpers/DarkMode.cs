using CincoVertice.Common.WinApi.Libs;
using CincoVertice.Common.WinApi.Libs.Enums;
using System.Security;
using Microsoft.Win32;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Windows' dark mode for apps, which the user chooses in Settings, Personalization, Colors; and what of a window
///     Windows itself draws dark: its title bar and its scroll bars. The rest is up to the app's colors.
/// </summary>
public static class DarkMode
{
    private const string PersonalizeKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

    /// <summary>Whether the user chose dark for apps; false when Windows doesn't say, as before Windows 10.</summary>
    public static bool IsOn()
    {
        try
        {
            using RegistryKey? key = Registry.CurrentUser.OpenSubKey(PersonalizeKey);

            return key?.GetValue("AppsUseLightTheme") is int light && light == 0;
        }
        catch (Exception ex) when (ex is SecurityException or UnauthorizedAccessException or IOException)
        {
            return false;
        }
    }

    /// <summary>Draws a window's title bar dark or light; does nothing where Windows can't.</summary>
    public static void UseDarkTitleBar(nint window, bool isDark)
    {
        int value = isDark ? 1 : 0;
        Dwmapi.DwmSetWindowAttribute(
            window,
            (int)DwmWindowAttribute.DWMWA_USE_IMMERSIVE_DARK_MODE,
            ref value,
            sizeof(int));
    }

    /// <summary>Draws a control's scroll bars dark or light, as Explorer does.</summary>
    public static void UseDarkScrollBars(nint control, bool isDark)
    {
        UxTheme.SetWindowTheme(control, isDark ? "DarkMode_Explorer" : "Explorer", null);
    }
}
