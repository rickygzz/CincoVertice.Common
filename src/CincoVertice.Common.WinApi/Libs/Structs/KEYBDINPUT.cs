using CincoVertice.Common.WinApi.Libs.Enums;

namespace CincoVertice.Common.WinApi.Libs.Structs;

/// <summary>
/// The information about a simulated keyboard event.
/// </summary>
public struct KEYBDINPUT
{
    /// <summary>
    ///     A virtual-key code. The code must be a value in the range 1 to 254. If the dwFlags member specifies
    ///     KEYEVENTF_UNICODE, wVk must be 0.
    /// </summary>
    public KeyCode WVk;

    /// <summary>
    ///     A hardware scan code for the key. If dwFlags specifies KEYEVENTF_UNICODE, wScan specifies a Unicode
    ///     character which is to be sent to the foreground application.
    /// </summary>
    public ushort WScan;

    /// <summary>Specifies various aspects of a keystroke.</summary>
    public KeyboardInputDwFlags DwFlags;

    /// <summary>
    ///     The time stamp for the event, in milliseconds. If this parameter is zero, the system will provide its
    ///     own time stamp.
    /// </summary>
    public uint Time;

    /// <summary>
    ///     An additional value associated with the keystroke. Use the GetMessageExtraInfo function to obtain this
    ///     information.
    /// </summary>
    public nint DwExtraInfo;
}
