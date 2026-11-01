using System.ComponentModel;
using System.Runtime.InteropServices;
using CincoVertice.Common.WinApi.Hotkey.Window;

namespace CincoVertice.Common.WinApi.Hotkey;

public class Hotkey : IDisposable
{
    private readonly HotkeyWindow _hotkeyWindow;
    private readonly List<HotkeyData> _hotkeys = [];

    private int _currentId;

    public Hotkey()
    {
        _hotkeyWindow = new HotkeyWindow();

        _hotkeyWindow.HotKeyPressed += Window_HotKeyPressed;
    }

    public void Dispose()
    {
        // unregister all the registered hot keys.
        for (int i = 0; i < _hotkeys.Count; i++)
        {
            _ = Libs.User32.UnregisterHotKey(_hotkeyWindow.Handle, _hotkeys[i].Id);
        }

        // Dispose the inner native window.
        _hotkeyWindow.Dispose();
    }

    /// <summary>
    ///     Registers a hot key in the system.
    /// </summary>
    /// <param name="hotkey">Hotkey.</param>
    /// <param name="error">Why it failed (e.g. another application uses the shortcut); empty on success.</param>
    /// <returns>True when the hot key was registered.</returns>
    public bool TryRegisterHotKey(HotkeyData hotkey, out string error)
    {
        _currentId++;
        hotkey.Id = _currentId;

        if (Libs.User32.RegisterHotKey(_hotkeyWindow.Handle, hotkey.Id, hotkey.Modifier, hotkey.Key) == 0)
        {
            error = new Win32Exception(Marshal.GetLastPInvokeError()).Message;

            return false;
        }

        _hotkeys.Add(hotkey);
        error = string.Empty;

        return true;
    }

    private void Window_HotKeyPressed(object? sender, HotkeyKeyPressedEventArgs args)
    {
        // Gets fired every time WM_HOTKEY is received
        // Look for the hotkey specific event to fire
        for (int i = 0; i < _hotkeys.Count; i++)
        {
            if (args.Id == _hotkeys[i].Id)
            {
                _hotkeys[i].Invoke(this, args);

                break;
            }
        }
    }
}
