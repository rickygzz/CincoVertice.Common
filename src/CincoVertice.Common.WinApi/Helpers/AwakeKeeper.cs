using System.ComponentModel;
using CincoVertice.Common.WinApi.Libs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Keeps the computer from sleeping and the screen on, while on, as a program showing a video does; even with the
///     screen locked. Nothing touches the mouse or the keyboard, so it doesn't stop the screen saver or the lock after
///     the time without input, and chat programs still see the user as away. Windows ties it to a thread: turn it on
///     and off on the same one, which must last, such as the UI thread; Windows ends it with the thread.
/// </summary>
public sealed class AwakeKeeper
{
    public bool IsOn { get; private set; }

    /// <exception cref="Win32Exception">When Windows refuses.</exception>
    public void TurnOn()
    {
        Kernel32.ExecutionState state = Kernel32.SetThreadExecutionState(
            Kernel32.ExecutionState.ES_CONTINUOUS
            | Kernel32.ExecutionState.ES_SYSTEM_REQUIRED
            | Kernel32.ExecutionState.ES_DISPLAY_REQUIRED);

        if (state == 0)
        {
            throw new Win32Exception("Windows refused to keep the computer awake.");
        }

        IsOn = true;
    }

    public void TurnOff()
    {
        if (IsOn)
        {
            Kernel32.SetThreadExecutionState(Kernel32.ExecutionState.ES_CONTINUOUS);
            IsOn = false;
        }
    }
}
