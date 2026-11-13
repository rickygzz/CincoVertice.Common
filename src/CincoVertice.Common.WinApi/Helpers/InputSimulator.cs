using CincoVertice.Common.WinApi.Libs.Enums;
using CincoVertice.Common.WinApi.Libs;
using CincoVertice.Common.WinApi.Libs.Structs;
using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Helpers;

public class InputSimulator
{
    // Keys that are sent as extended: those that share a scan code with a numpad key, and the right Alt and Ctrl,
    // which are otherwise the left ones
    private static readonly KeyCode[] _extendedKeys =
    [
        KeyCode.VK_INSERT, KeyCode.VK_DELETE, KeyCode.VK_HOME, KeyCode.VK_END, KeyCode.VK_PRIOR, KeyCode.VK_NEXT,
        KeyCode.VK_LEFT, KeyCode.VK_UP, KeyCode.VK_RIGHT, KeyCode.VK_DOWN, KeyCode.VK_DIVIDE, KeyCode.VK_SNAPSHOT,
        KeyCode.VK_LWIN, KeyCode.VK_RWIN, KeyCode.VK_APPS, KeyCode.VK_RMENU, KeyCode.VK_RCONTROL,
    ];

    private readonly List<INPUT> toSend = [];

    /// <summary>
    ///     SendInputs added with AddKey().
    /// </summary>
    public void SendInputs()
    {
        if (this.toSend.Count > 0)
        {
            INPUT[] inputs = [.. this.toSend];
            User32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            this.toSend.Clear();
        }
    }

    /// <summary>
    ///     Add click
    /// </summary>
    /// <param name="mouseFlags">Mouse Flags</param>
    public void AddClick(MouseInputDwFlags mouseFlags)
    {
        this.toSend.Add(MouseInputClick(mouseFlags));
    }

    /// <summary>
    ///     Add click
    /// </summary>
    public void AddClick()
    {
        this.toSend.Add(MouseInputClick(MouseInputDwFlags.MOUSEEVENTF_LEFTDOWN));
        this.toSend.Add(MouseInputClick(MouseInputDwFlags.MOUSEEVENTF_LEFTUP));
    }

    /// <summary>
    ///     Add wheel movement
    /// </summary>
    /// <param name="movementAmount">Amount of wheel movement</param>
    public void AddWheel(int movementAmount)
    {
        this.toSend.Add(MouseInputWheel(movementAmount));
    }

    /// <summary>
    ///     Simulate key.
    /// </summary>
    /// <param name="key">key.</param>
    /// <param name="kbFlags">kbFlags.</param>
    public void AddKey(KeyCode key, KeyboardInputDwFlags kbFlags)
    {
        this.toSend.Add(KeyboardInput(key, kbFlags));
    }

    /// <summary>
    ///     Simulate a key.
    /// </summary>
    /// <param name="key">key.</param>
    /// <param name="modifier">! ALT, ^ CTL, + SHF, # WIN or any combination.</param>
    public void AddKey(KeyCode key, string modifier = "")
    {
        foreach (char c in modifier)
        {
            switch (c)
            {
                case '^':
                    this.toSend.Add(KeyboardInput(KeyCode.VK_CONTROL, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
                    break;
                case '!':
                    this.toSend.Add(KeyboardInput(KeyCode.VK_MENU, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
                    break;
                case '#':
                    this.toSend.Add(KeyboardInput(KeyCode.VK_LWIN, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
                    break;
                case '+':
                    this.toSend.Add(KeyboardInput(KeyCode.VK_SHIFT, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
                    break;
            }
        }

        this.toSend.Add(KeyboardInput(key, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
        this.toSend.Add(KeyboardInput(key, KeyboardInputDwFlags.KEYEVENTF_KEYUP));

        foreach (char c in modifier)
        {
            switch (c)
            {
                case '^':
                    this.toSend.Add(KeyboardInput(KeyCode.VK_CONTROL, KeyboardInputDwFlags.KEYEVENTF_KEYUP));
                    break;
                case '!':
                    this.toSend.Add(KeyboardInput(KeyCode.VK_MENU, KeyboardInputDwFlags.KEYEVENTF_KEYUP));
                    break;
                case '#':
                    this.toSend.Add(KeyboardInput(KeyCode.VK_LWIN, KeyboardInputDwFlags.KEYEVENTF_KEYUP));
                    break;
                case '+':
                    this.toSend.Add(KeyboardInput(KeyCode.VK_SHIFT, KeyboardInputDwFlags.KEYEVENTF_KEYUP));
                    break;
            }
        }
    }

    /// <summary>
    ///     SendText.
    /// </summary>
    /// <param name="text">text.
    ///     <para>% ALT.</para>
    ///     <para>^ CTL.</para>
    ///     <para>+ SHIFT.</para>
    ///     <para>~ WIN.</para>
    /// </param>
    /// <param name="milliseconds">milliseconds.</param>
    public static void SendText(string text, int milliseconds = 0)
    {
        text = text
            .Replace("(", "{(}")
            .Replace(")", "{)}")
            .Replace(Environment.NewLine, "+{ENTER}")
            .Replace("\x0A", "+{ENTER}")
            .Replace("\x0C", "+{ENTER}");

        // text.Replace("+", "{+}").Replace("^", "{^}").Replace("%", "{%}").Replace("~", "{~}")
        // try
        // {
        //    System.Windows.Forms.SendKeys.SendWait(text);
        // }
        // catch (Exception ex)
        // {
        //    //MC.Message.Error("SendText()", ex.Message);
        // }

        List<INPUT> textToSend = [];

        foreach (char key in text)
        {
            textToSend.Add(KeyboardInput((KeyCode)key, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
            textToSend.Add(KeyboardInput((KeyCode)key, KeyboardInputDwFlags.KEYEVENTF_KEYUP));

            INPUT[] inputs = [.. textToSend];

            User32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

            Thread.Sleep(milliseconds);

            textToSend.Clear();
        }
    }

    /// <summary>
    ///     Simulates a user click.
    /// </summary>
    public static void Click(bool sendToChildUnderCursor = false)
    {
        if (sendToChildUnderCursor)
        {
            User32.GetCursorPos(out POINT point);

            nint childUnderCursor = WinUser.WindowFromPoint(point);

            if (User32.GetWindowRect(childUnderCursor, out RECT rec) == 0)
            {
                return;
            }

            int newx = point.X - rec.Left;
            int newy = point.Y - rec.Top;

            WinUser.SendMessage(childUnderCursor, WM.WM_LBUTTONDOWN, 0x0001, (newy << 16) | newx);
            WinUser.SendMessage(childUnderCursor, WM.WM_LBUTTONUP, 0x0001, (newy << 16) | newx);
        }
        else
        {
            INPUT mi1 = MouseInputClick(MouseInputDwFlags.MOUSEEVENTF_LEFTDOWN);
            INPUT mi2 = MouseInputClick(MouseInputDwFlags.MOUSEEVENTF_LEFTUP);

            INPUT[] mouseInputs = new INPUT[] { mi1, mi2 };
            User32.SendInput((uint)mouseInputs.Length, mouseInputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }

    /// <summary>
    ///     Simulates a keypress.
    /// </summary>
    /// <param name="key">key.</param>
    /// <param name="ctlPressed">Is control key pressed?.</param>
    /// <param name="altPressed">Is alt key pressed?.</param>
    public static void SendKey(KeyCode key, bool ctlPressed = false, bool altPressed = false)
    {
        List<INPUT> list = [];

        if (ctlPressed)
        {
            list.Add(KeyboardInput(KeyCode.VK_CONTROL, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
        }

        if (altPressed)
        {
            list.Add(KeyboardInput(KeyCode.VK_MENU, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
        }

        list.Add(KeyboardInput(key, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
        list.Add(KeyboardInput(key, KeyboardInputDwFlags.KEYEVENTF_KEYUP));

        if (ctlPressed)
        {
            list.Add(KeyboardInput(KeyCode.VK_CONTROL, KeyboardInputDwFlags.KEYEVENTF_KEYUP));
        }

        if (altPressed)
        {
            list.Add(KeyboardInput(KeyCode.VK_MENU, KeyboardInputDwFlags.KEYEVENTF_KEYUP));
        }

        INPUT[] kbinputs = [.. list];
        User32.SendInput((uint)kbinputs.Length, kbinputs, Marshal.SizeOf(typeof(INPUT)));
    }

    /// <summary>
    ///     Presses a shortcut such as Ctrl+V, <paramref name="times"/> times: the modifiers down, the key down and
    ///     up, then the modifiers up.
    /// </summary>
    /// <param name="modifiers">The modifiers to hold.</param>
    /// <param name="key">The key to press.</param>
    /// <param name="times">How many times to press it.</param>
    /// <param name="altGr">True to hold AltGr too, as in <see cref="ShortcutInputs"/>.</param>
    /// <exception cref="InvalidOperationException">Windows didn't take all the keys.</exception>
    public static void SendShortcut(FSModifiers modifiers, KeyCode key, int times = 1, bool altGr = false)
    {
        Send(ShortcutInputs(modifiers, key, times, altGr));
    }

    /// <summary>
    ///     The keys <see cref="SendShortcut"/> sends. AltGr is Right Alt with Left Ctrl held first, as a keyboard
    ///     with AltGr sends it, so it works whether or not Windows adds the Ctrl itself.
    /// </summary>
    public static INPUT[] ShortcutInputs(FSModifiers modifiers, KeyCode key, int times = 1, bool altGr = false)
    {
        List<KeyCode> modifierKeys = [];

        if (altGr)
        {
            modifierKeys.Add(KeyCode.VK_LCONTROL);
            modifierKeys.Add(KeyCode.VK_RMENU);
        }

        if (modifiers.HasFlag(FSModifiers.MOD_CONTROL))
        {
            modifierKeys.Add(KeyCode.VK_CONTROL);
        }

        if (modifiers.HasFlag(FSModifiers.MOD_ALT))
        {
            modifierKeys.Add(KeyCode.VK_MENU);
        }

        if (modifiers.HasFlag(FSModifiers.MOD_SHIFT))
        {
            modifierKeys.Add(KeyCode.VK_SHIFT);
        }

        if (modifiers.HasFlag(FSModifiers.MOD_WIN))
        {
            modifierKeys.Add(KeyCode.VK_LWIN);
        }

        List<INPUT> list = [];

        foreach (var modifierKey in modifierKeys)
        {
            list.Add(KeyboardInput(modifierKey, ExtendedFlag(modifierKey)));
        }

        for (int i = 0; i < times; i++)
        {
            list.Add(KeyboardInput(key, ExtendedFlag(key)));
            list.Add(KeyboardInput(key, ExtendedFlag(key) | KeyboardInputDwFlags.KEYEVENTF_KEYUP));
        }

        for (int i = modifierKeys.Count - 1; i >= 0; i--)
        {
            var keyUp = ExtendedFlag(modifierKeys[i]) | KeyboardInputDwFlags.KEYEVENTF_KEYUP;
            list.Add(KeyboardInput(modifierKeys[i], keyUp));
        }

        return [.. list];
    }

    /// <summary>
    ///     Types the text into the window that has the keyboard, whatever its keyboard layout, as
    ///     <see cref="UnicodeTextInputs"/> does.
    /// </summary>
    /// <exception cref="InvalidOperationException">Windows didn't take all the keys.</exception>
    public static void SendUnicodeText(string text)
    {
        Send(UnicodeTextInputs(text));
    }

    /// <summary>
    ///     The keys that type the text: each character as itself (KEYEVENTF_UNICODE), so the window gets "@" and not
    ///     the key that types "@" on a layout, which on its own layout may type something else or nothing. A character
    ///     beyond the first 65536, such as an emoji, goes as its two UTF-16 halves. A new line (\n, \r or \r\n) is
    ///     Enter and \t is Tab, as real keys, since programs act on those keys rather than on their characters.
    /// </summary>
    public static INPUT[] UnicodeTextInputs(string text)
    {
        List<INPUT> list = new(text.Length * 2);

        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];

            switch (c)
            {
                case '\r' or '\n':
                    AddKeyPress(list, KeyCode.VK_RETURN);

                    // \r\n is one new line
                    if (c == '\r' && i + 1 < text.Length && text[i + 1] == '\n')
                    {
                        i++;
                    }

                    break;

                case '\t':
                    AddKeyPress(list, KeyCode.VK_TAB);
                    break;

                default:
                    list.Add(UnicodeInput(c, KeyboardInputDwFlags.KEYEVENTF_UNICODE));
                    list.Add(UnicodeInput(
                        c,
                        KeyboardInputDwFlags.KEYEVENTF_UNICODE | KeyboardInputDwFlags.KEYEVENTF_KEYUP));
                    break;
            }
        }

        return [.. list];
    }

    private static void AddKeyPress(List<INPUT> list, KeyCode key)
    {
        list.Add(KeyboardInput(key, KeyboardInputDwFlags.KEYEVENTF_KEYDOWN));
        list.Add(KeyboardInput(key, KeyboardInputDwFlags.KEYEVENTF_KEYUP));
    }

    private static INPUT UnicodeInput(char c, KeyboardInputDwFlags flags) => new()
    {
        Type = InputType.INPUT_KEYBOARD,
        MKHInput = new MOUSEKEYBDHARDWAREINPUT()
        {
            KeyboardInput = new KEYBDINPUT()
            {
                WVk = 0,
                WScan = c,
                DwFlags = flags,
                Time = 0,
                DwExtraInfo = User32.GetMessageExtraInfo()
            }
        }
    };

    /// <summary>
    ///     Sends the keys in one go, so nothing typed on the keyboard meanwhile lands between them.
    /// </summary>
    /// <exception cref="InvalidOperationException">Windows didn't take all of them.</exception>
    private static void Send(INPUT[] inputs)
    {
        if (inputs.Length == 0)
        {
            return;
        }

        uint sent = User32.SendInput((uint)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));

        if (sent != inputs.Length)
        {
            throw new InvalidOperationException(
                $"Windows took {sent} of the {inputs.Length} key presses; another program may be blocking them.");
        }
    }

    private static KeyboardInputDwFlags ExtendedFlag(KeyCode key) =>
        _extendedKeys.Contains(key)
        ? KeyboardInputDwFlags.KEYEVENTF_EXTENDEDKEY
        : KeyboardInputDwFlags.KEYEVENTF_KEYDOWN;

    /// <summary>
    ///     Simulates wheel scroll
    /// </summary>
    /// <param name="times">
    ///     Amount of wheel movement. A positive value indicates that the wheel was rotated forward,
    ///     away from the user; a negative value indicates that the wheel was rotated backward, toward the user.
    /// </param>
    public static void Wheel(int times)
    {
        INPUT[] mouseInputs = [MouseInputWheel(times)];
        User32.SendInput((uint)mouseInputs.Length, mouseInputs, Marshal.SizeOf(typeof(INPUT)));
    }

    /// <summary>
    ///     MouseInputClick
    /// </summary>
    /// <param name="mouseFlags">LEFT/MIDDLE/RIGHT UP/DOWN</param>
    /// <returns>Returns INPUT information for SendInput() events</returns>
    public static INPUT MouseInputClick(MouseInputDwFlags mouseFlags)
    {
        INPUT minput = new()
        {
            Type = InputType.INPUT_MOUSE,
            MKHInput = new MOUSEKEYBDHARDWAREINPUT()
            {
                MouseInput = new MOUSEINPUT()
                {
                    Dx = 0,
                    Dy = 0,
                    MouseData = MouseInputMouseData.NONE,
                    DwFlags = mouseFlags,
                    Time = 0,
                    DwExtraInfo = User32.GetMessageExtraInfo()
                }
            }
        };

        return minput;
    }

    /// <summary>
    ///     MouseInputWheel
    /// </summary>
    /// <param name="movementAmount">
    ///     Amount of wheel movement. A positive value indicates that the wheel was rotated forward, away
    ///     from the user; a negative value indicates that the wheel was rotated backward, toward the user.
    /// </param>
    /// <returns>
    ///     Returns INPUT information for SendInput() events
    /// </returns>
    public static INPUT MouseInputWheel(int movementAmount)
    {
        INPUT minput = new()
        {
            Type = InputType.INPUT_MOUSE,
            MKHInput = new MOUSEKEYBDHARDWAREINPUT()
            {
                MouseInput = new MOUSEINPUT()
                {
                    Dx = 0,
                    Dy = 0,
                    MouseData = (MouseInputMouseData)(160 * movementAmount),
                    DwFlags = MouseInputDwFlags.MOUSEEVENTF_WHEEL,
                    Time = 0,
                    DwExtraInfo = User32.GetMessageExtraInfo()
                }
            }
        };

        return minput;
    }

    /// <summary>
    ///     MouseInputMoveDelta
    /// </summary>
    /// <param name="deltaX">delta X</param>
    /// <param name="deltaY">delta Y</param>
    /// <returns>Returns INPUT information for SendInput() events</returns>
    public static INPUT MouseInputMoveDelta(int deltaX, int deltaY)
    {
        INPUT minput = new()
        {
            Type = InputType.INPUT_MOUSE,
            MKHInput = new MOUSEKEYBDHARDWAREINPUT()
            {
                MouseInput = new MOUSEINPUT()
                {
                    Dx = deltaX,
                    Dy = deltaY,
                    MouseData = MouseInputMouseData.NONE,
                    DwFlags = MouseInputDwFlags.MOUSEEVENTF_MOVE,
                    Time = 0,
                    DwExtraInfo = User32.GetMessageExtraInfo()
                }
            }
        };

        return minput;
    }

    /// <summary>
    ///     KeyboardInput
    /// </summary>
    /// <param name="key">key</param>
    /// <param name="kbFlags">flags</param>
    /// <returns>Returns INPUT information for SendInput() events</returns>
    public static INPUT KeyboardInput(KeyCode key, KeyboardInputDwFlags kbFlags)
    {
        INPUT kbinput = new()
        {
            Type = InputType.INPUT_KEYBOARD,
            MKHInput = new MOUSEKEYBDHARDWAREINPUT()
            {
                KeyboardInput = new KEYBDINPUT()
                {
                    WVk = key,
                    DwFlags = kbFlags,
                    Time = 0,
                    DwExtraInfo = User32.GetMessageExtraInfo()
                }
            }
        };

        return kbinput;
    }

    public static void MouseMove(int x, int y)
    {
        User32.SetCursorPos(x, y);
    }

    /// <summary>
    ///     Simulate user mouse move from starting point to destination point.
    /// </summary>
    /// <param name="startX">If startX and startY = -1, gets the current cursor position.</param>
    /// <param name="startY">If startY and startX = -1, gets the current cursor position.</param>
    /// <param name="endX">endX.</param>
    /// <param name="endY">endY.</param>
    /// <param name="speed">
    ///     Value between 20 and 100 milliseconds. The greater, the slower the cursor will move.
    /// </param>
    public static void IncrementalMouseMove(int startX, int startY, int endX, int endY, int speed)
    {
        if ((startX == -1) && (startY == -1))
        {
            User32.GetCursorPos(out POINT pos);

            startX = pos.X;
            startY = pos.Y;
        }

        // Ensure speed is between 20 and 100 milliseconds.
        if (speed > 100)
        {
            speed = 100;
        }
        else if (speed < 20)
        {
            speed = 20;
        }

        // prepare to calculate increments (mouse movements)
        double deltaX = endX - startX;
        double deltaY = endY - startY;

        double increaseX = 0;
        double increaseY = 0;

        if (deltaX != 0)
        {
            increaseX = deltaX / 10;
        }

        if (deltaY != 0)
        {
            increaseY = deltaY / 10;
        }

        double newX = startX;
        double newY = startY;

        while ((startX != endX) || (startY != endY))
        {
            newX += increaseX;
            newY += increaseY;

            if ((deltaX < 0 && (newX - endX) < 0.01) || (deltaX > 0 && (newX - endX) > 0.01))
            {
                startX = endX;
            }
            else
            {
                startX = (int)newX;
            }

            if ((deltaY < 0 && (newY - endY) < 0.01) || (deltaY > 0 && (newY - endY) > 0.01))
            {
                startY = endY;
            }
            else
            {
                startY = (int)newY;
            }

            User32.SetCursorPos(startX, startY);

            Thread.Sleep(speed);
        }
    }

    /// <summary>
    ///     Waits until Control, Alt, Shift and Win keys are Up.
    /// </summary>
    public static void WaitUntilAllModifiersAreUp()
    {
        WaitUntilControlIsUp();
        WaitUntilALTIsUp();
        WaitUntilShiftIsUp();
        WaitUntilWinIsUp();
    }

    /// <summary>
    ///     Waits until Control key is up.
    /// </summary>
    public static void WaitUntilControlIsUp()
    {
        short state;

        do
        {
            state = User32.GetAsyncKeyState(KeyCode.VK_CONTROL);
        } while (((1 << 15) & state) != 0);
    }

    /// <summary>
    ///     Waits until Alt key is up.
    /// </summary>
    public static void WaitUntilALTIsUp()
    {
        short state;

        do
        {
            state = User32.GetAsyncKeyState(KeyCode.VK_MENU);
        } while (((1 << 15) & state) != 0);
    }

    /// <summary>
    ///     Waits until shift is up.
    /// </summary>
    public static void WaitUntilShiftIsUp()
    {
        short state;

        do
        {
            state = User32.GetAsyncKeyState(KeyCode.VK_SHIFT);
        } while (((1 << 15) & state) != 0);
    }

    /// <summary>
    ///     Waits until win is up.
    /// </summary>
    public static void WaitUntilWinIsUp()
    {
        short state;

        do
        {
            state = User32.GetAsyncKeyState(KeyCode.VK_LWIN);
        } while (((1 << 15) & state) != 0);

        do
        {
            state = User32.GetAsyncKeyState(KeyCode.VK_RWIN);
        } while (((1 << 15) & state) != 0);
    }
}
