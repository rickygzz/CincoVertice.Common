using CincoVertice.Common.WinApi.Libs.Enums;
using CincoVertice.Common.WinApi.Libs;
using CincoVertice.Common.WinApi.Libs.Structs;
using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Helpers;

public class InputSimulator
{
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
        //try
        //{
        //    System.Windows.Forms.SendKeys.SendWait(text);
        //}
        //catch (Exception ex)
        //{
        //    //MC.Message.Error("SendText()", ex.Message);
        //}

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
