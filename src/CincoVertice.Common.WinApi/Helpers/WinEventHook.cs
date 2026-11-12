using System.Runtime.InteropServices;
using CincoVertice.Common.WinApi.Libs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Tells of window events in other processes, such as the user starting to drag a window, until disposed. The
///     calls come through the message loop of the thread that creates it, so create it on the UI thread.
/// </summary>
public sealed class WinEventHook : IDisposable
{
    // Windows calls it until the hook is removed, so it must stay alive that long
    private readonly User32.WinEventProc _proc;
    private readonly Action<uint, nint> _callback;

    private nint _hook;

    /// <param name="eventMin">The first event, e.g. <see cref="User32.EVENT_SYSTEM_MOVESIZESTART"/>.</param>
    /// <param name="eventMax">The last event.</param>
    /// <param name="callback">Called with the event and its top-level window, for events of whole windows only.</param>
    /// <exception cref="InvalidOperationException">When Windows refuses the hook.</exception>
    public WinEventHook(uint eventMin, uint eventMax, Action<uint, nint> callback)
    {
        _callback = callback;
        _proc = OnEvent;
        _hook = User32.SetWinEventHook(
            eventMin,
            eventMax,
            0,
            Marshal.GetFunctionPointerForDelegate(_proc),
            0,
            0,
            User32.WINEVENT_OUTOFCONTEXT | User32.WINEVENT_SKIPOWNPROCESS);

        if (_hook == 0)
        {
            throw new InvalidOperationException("Windows refused to tell of window events.");
        }
    }

    public void Dispose()
    {
        if (_hook != 0)
        {
            User32.UnhookWinEvent(_hook);
            _hook = 0;
        }
    }

    private void OnEvent(
        nint hook,
        uint eventType,
        nint hwnd,
        int idObject,
        int idChild,
        uint eventThread,
        uint eventTime)
    {
        if (hwnd != 0 && idObject == User32.OBJID_WINDOW && idChild == 0)
        {
            _callback(eventType, hwnd);
        }
    }
}
