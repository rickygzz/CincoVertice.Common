using System.ComponentModel;
using System.Runtime.InteropServices;
using CincoVertice.Common.WinApi.Libs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Tells of the clicks and key presses meant for any program, until disposed, without changing them. The calls
///     come through the messages of the thread that creates it, so create it on the UI thread, and keep the handlers
///     short: Windows waits for them before the input goes on.
/// </summary>
public sealed class InputHook : IDisposable
{
    // Windows calls them until the hooks are removed, so they must stay alive that long
    private readonly User32.LowLevelHookProc _mouseProc;
    private readonly User32.LowLevelHookProc _keyboardProc;

    private nint _mouseHook;
    private nint _keyboardHook;

    /// <exception cref="Win32Exception">When Windows refuses the hooks.</exception>
    public InputHook()
    {
        _mouseProc = OnMouse;
        _keyboardProc = OnKeyboard;
        nint module = Kernel32.GetModuleHandle(null!);

        _mouseHook = User32.SetWindowsHookEx(
            User32.WH_MOUSE_LL,
            Marshal.GetFunctionPointerForDelegate(_mouseProc),
            module,
            0);
        _keyboardHook = User32.SetWindowsHookEx(
            User32.WH_KEYBOARD_LL,
            Marshal.GetFunctionPointerForDelegate(_keyboardProc),
            module,
            0);

        if (_mouseHook == 0 || _keyboardHook == 0)
        {
            int error = Marshal.GetLastPInvokeError();
            Dispose();

            throw new Win32Exception(error);
        }
    }

    /// <summary>Raised when a mouse button goes down, with where, in physical screen pixels.</summary>
    public event EventHandler<(Point Point, MouseButtons Button)>? MouseDown;

    /// <summary>Raised when a key goes down, with its virtual key code.</summary>
    public event EventHandler<Keys>? KeyDown;

    public void Dispose()
    {
        if (_mouseHook != 0)
        {
            User32.UnhookWindowsHookEx(_mouseHook);
            _mouseHook = 0;
        }

        if (_keyboardHook != 0)
        {
            User32.UnhookWindowsHookEx(_keyboardHook);
            _keyboardHook = 0;
        }
    }

    private nint OnMouse(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= 0)
        {
            MouseButtons button = (int)wParam switch
            {
                User32.WM_LBUTTONDOWN => MouseButtons.Left,
                User32.WM_RBUTTONDOWN => MouseButtons.Right,
                User32.WM_MBUTTONDOWN => MouseButtons.Middle,
                _ => MouseButtons.None,
            };

            if (button != MouseButtons.None)
            {
                var input = Marshal.PtrToStructure<User32.MSLLHOOKSTRUCT>(lParam);
                MouseDown?.Invoke(this, (new Point(input.pt.X, input.pt.Y), button));
            }
        }

        return User32.CallNextHookEx(_mouseHook, nCode, wParam, lParam);
    }

    private nint OnKeyboard(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= 0 && (int)wParam is User32.WM_KEYDOWN or User32.WM_SYSKEYDOWN)
        {
            var input = Marshal.PtrToStructure<User32.KBDLLHOOKSTRUCT>(lParam);
            KeyDown?.Invoke(this, (Keys)input.vkCode);
        }

        return User32.CallNextHookEx(_keyboardHook, nCode, wParam, lParam);
    }
}
