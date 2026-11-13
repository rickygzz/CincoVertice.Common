using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;

public static partial class UxTheme
{
    /// <summary>
    ///     Draws a window's standard parts, such as its scroll bars, with another visual style: "DarkMode_Explorer"
    ///     draws them dark, as Explorer does in dark mode.
    /// </summary>
    /// <param name="hwnd">The window.</param>
    /// <param name="pszSubAppName">The style's application, e.g. "DarkMode_Explorer"; null for the usual one.</param>
    /// <param name="pszSubIdList">The style's class names; null for the window's own.</param>
    /// <returns>0 when it worked; an HRESULT error code otherwise.</returns>
    [LibraryImport("uxtheme.dll", StringMarshalling = StringMarshalling.Utf16)]
    public static partial int SetWindowTheme(nint hwnd, string? pszSubAppName, string? pszSubIdList);
}
