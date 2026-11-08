using CincoVertice.Common.WinApi.Libs.Enums;

namespace CincoVertice.Common.WinApi.Libs.Structs;

/// <summary>
///     Contains information about a simulated mouse event.
/// </summary>
public struct MOUSEINPUT
{
    /// <summary>
    ///     The absolute position of the mouse, or the amount of motion since the last mouse event was generated,
    ///     depending on the value of the dwFlags member. Absolute data is specified as the x coordinate of the
    ///     mouse; relative data is specified as the number of pixels moved.
    /// </summary>
    public int Dx;

    /// <summary>
    ///     The absolute position of the mouse, or the amount of motion since the last mouse event was generated,
    ///     depending on the value of the dwFlags member. Absolute data is specified as the y coordinate of the
    ///     mouse; relative data is specified as the number of pixels moved.
    /// </summary>
    public int Dy;

    /// <summary>Flags depending on DwFlags value</summary>
    public MouseInputMouseData MouseData;

    /// <summary>
    ///     A set of bit flags that specify various aspects of mouse motion and button clicks. The bits in this
    ///     member can be any reasonable combination of the following values.
    ///     <para>
    ///         The bit flags that specify mouse button status are set to indicate changes in status, not ongoing
    ///         conditions. For example, if the left mouse button is pressed and held down, MOUSEEVENTF_LEFTDOWN
    ///         is set when the left button is first pressed, but not for subsequent motions. Similarly,
    ///         MOUSEEVENTF_LEFTUP is set only when the button is first released.
    ///     </para>
    ///     <para>
    ///         You cannot specify both the MOUSEEVENTF_WHEEL flag and either MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP
    ///         flags simultaneously in the dwFlags parameter, because they both require use of the mouseData field.
    ///     </para>
    /// </summary>
    public MouseInputDwFlags DwFlags;

    /// <summary>
    ///     The time stamp for the event, in milliseconds. If this parameter is 0, the system will provide its own
    ///     time stamp.
    /// </summary>
    public uint Time;

    /// <summary>
    ///     An additional value associated with the mouse event. An application calls GetMessageExtraInfo to obtain
    ///     this extra information.
    /// </summary>
    public nint DwExtraInfo;
}
