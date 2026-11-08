namespace CincoVertice.Common.WinApi.Hotkey.Window;

public class HotkeyWindow : NativeWindow, IDisposable
{
    public event EventHandler<HotkeyKeyPressedEventArgs> HotKeyPressed;

    /// <summary>
    ///     Initializes a new instance of the <see cref="HotkeyWindow"/> class.
    /// </summary>
    public HotkeyWindow()
    {
        HotKeyPressed += delegate { };

        // Creates a window and its handle with the specified creation parameters.
        CreateHandle(new CreateParams());
    }

    public void Dispose()
    {
        DestroyHandle();
    }

    /// <summary>
    ///     Invokes the default window procedure associated with this window.
    /// </summary>
    /// <param name="m">A Message that is associated with the current Windows message.</param>
    protected override void WndProc(ref System.Windows.Forms.Message m)
    {
        base.WndProc(ref m);

        // check if we got a hot key pressed.
        // WM_HOTKEY HOTKEY Windows Message.
        if (m.Msg == 0x0312)
        {
            // Get ID
            int id = (int)m.WParam;

            // Get the keys.
            Libs.Enums.KeyCode key = (Libs.Enums.KeyCode)(((int)m.LParam >> 16) & 0xFFFF);
            Libs.Enums.FSModifiers modifier = (Libs.Enums.FSModifiers)((int)m.LParam & 0xFFFF);

            // Invoke the event to notify the parent
            HotKeyPressed?.Invoke(this, new HotkeyKeyPressedEventArgs(id, modifier, key));
        }
    }
}
