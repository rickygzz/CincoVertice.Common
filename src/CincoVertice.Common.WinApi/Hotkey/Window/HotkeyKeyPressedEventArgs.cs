namespace CincoVertice.Common.WinApi.Hotkey.Window;

public class HotkeyKeyPressedEventArgs
{
    public HotkeyKeyPressedEventArgs(int id, Libs.Enums.FSModifiers modifier, Libs.Enums.KeyCode key)
    {
        Id = id;
        Modifier = modifier;
        Key = key;
    }

    public int Id { get; private set; }

    public Libs.Enums.FSModifiers Modifier { get; private set; }

    public Libs.Enums.KeyCode Key { get; private set; }

    public static readonly HotkeyKeyPressedEventArgs Empty = new(0, 0, 0);
}
