using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs.Structs;

/// <summary>
///     Union { MOUSEINPUT KEYBOARDINPUT and HARDWAREINPUT }
///     Used in User32.SendInput()
/// </summary>
[StructLayout(LayoutKind.Explicit)]
public struct MOUSEKEYBDHARDWAREINPUT
{
    /// <summary>The information about a simulated mouse event.</summary>
    [FieldOffset(0)]
    public MOUSEINPUT MouseInput;

    /// <summary>The information about a simulated keyboard event.</summary>
    [FieldOffset(0)]
    public KEYBDINPUT KeyboardInput;

    /// <summary>The information about a simulated hardware event.</summary>
    [FieldOffset(0)]
    public HARDWAREINPUT HardwareInput;
}
