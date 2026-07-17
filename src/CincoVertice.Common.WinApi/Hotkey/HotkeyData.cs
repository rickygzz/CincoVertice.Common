using CincoVertice.Common.WinApi.Hotkey.Window;
using CincoVertice.Common.WinApi.Libs.Enums;

namespace CincoVertice.Common.WinApi.Hotkey;

public class HotkeyData
{
    public int Id { get; set; }

    public FSModifiers Modifier { get; set; }

    public KeyCode Key { get; set; }

    public event EventHandler<HotkeyKeyPressedEventArgs> KeyPressed;

    public HotkeyData(
        FSModifiers modifier,
        KeyCode key,
        EventHandler<HotkeyKeyPressedEventArgs> keyPressed)
    {
        Id = 0;
        Modifier = modifier;
        Key = key;
        KeyPressed = keyPressed;
    }

    public void Invoke(object sender, HotkeyKeyPressedEventArgs args)
    {
        KeyPressed?.Invoke(sender, args);
    }
}
