using CincoVertice.Common.WinApi.Helpers;
using CincoVertice.Common.WinApi.Hotkey.Window;
using System.Runtime.Versioning;

namespace CincoVertice.Common.WinApi.Hotkey;

[SupportedOSPlatform("windows10.0")]
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
    public void RegisterHotKey(HotkeyData hotkey)
    {
        // Increment the counter.
        _currentId++;

        hotkey.Id = _currentId;

        // Register the hot key.
        if (Libs.User32.RegisterHotKey(_hotkeyWindow.Handle, hotkey.Id, hotkey.Modifier, hotkey.Key) == 0)
        {
            MessageHelper.ShowLastWin32Error("RegisterHotKey() key " + hotkey.Key.ToString() + " modifier" + hotkey.Modifier.ToString());
        }
        else
        {
            _hotkeys.Add(hotkey);
        }
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
