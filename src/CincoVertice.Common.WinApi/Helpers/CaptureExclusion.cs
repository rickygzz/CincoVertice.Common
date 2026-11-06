using CincoVertice.Common.WinApi.Libs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Keeps windows out of screenshots for a while, as <see cref="ScreenCapture"/> takes them, while they stay on the
///     monitors as they are: nothing moves or flickers. Only for a while, as it also keeps them out of screen sharing
///     and recordings.
/// </summary>
public sealed class CaptureExclusion : IDisposable
{
    private readonly List<nint> _windows;

    private CaptureExclusion(List<nint> windows)
    {
        _windows = windows;
    }

    /// <summary>Leaves these windows out of screenshots until the result is disposed.</summary>
    /// <param name="windows">Top-level windows of this process.</param>
    /// <returns>
    ///     What brings them back; null when none could be left out, as before Windows 10, version 2004, so the caller can
    ///     hide them some other way.
    /// </returns>
    public static CaptureExclusion? Exclude(IEnumerable<nint> windows)
    {
        List<nint> excluded =
        [
            .. windows.Where(window => window != 0
                && User32.SetWindowDisplayAffinity(window, User32.DisplayAffinity.WDA_EXCLUDEFROMCAPTURE)),
        ];

        if (excluded.Count == 0)
        {
            return null;
        }

        // The screen keeps them until it's drawn again
        Dwmapi.DwmFlush();

        return new CaptureExclusion(excluded);
    }

    public void Dispose()
    {
        foreach (nint window in _windows)
        {
            User32.SetWindowDisplayAffinity(window, User32.DisplayAffinity.WDA_NONE);
        }

        _windows.Clear();
    }
}
