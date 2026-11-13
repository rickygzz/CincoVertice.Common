using CincoVertice.Common.WinApi.Libs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Makes the calling thread per-monitor DPI aware until disposed, then restores its previous awareness. Use it
///     around code that needs physical pixels on every monitor (e.g. capturing the screen) in an application that is
///     only system aware. Windows created inside the scope stay per-monitor aware after it ends.
/// </summary>
public sealed class DpiAwarenessScope : IDisposable
{
    private readonly nint _previous;

    private DpiAwarenessScope(nint context)
    {
        _previous = User32.SetThreadDpiAwarenessContext(context);
    }

    public static DpiAwarenessScope PerMonitorV2() => new(User32.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

    public void Dispose()
    {
        if (_previous != 0)
        {
            User32.SetThreadDpiAwarenessContext(_previous);
        }
    }
}
