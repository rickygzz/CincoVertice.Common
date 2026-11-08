namespace CincoVertice.Common.WinApi.Libs.Enums;

/// <summary>
///     Flags for MouseInput
/// </summary>
public enum MouseInputMouseData : uint
{
    /// <summary>
    ///     If dwFlags does not contain MOUSEEVENTF_WHEEL, MOUSEEVENTF_XDOWN, or MOUSEEVENTF_XUP, then mouseData
    ///     should be zero.
    /// </summary>
    NONE = 0,

    /// <summary>
    ///     If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were
    ///     pressed or released. Set if the first X button is pressed or released.
    /// </summary>
    XBUTTON1 = 1,

    /// <summary>
    ///     If dwFlags contains MOUSEEVENTF_XDOWN or MOUSEEVENTF_XUP, then mouseData specifies which X buttons were
    ///     pressed or released. Set if the second X button is pressed or released.
    /// </summary>
    XBUTTON2 = 2,

    /// <summary>
    ///     If dwFlags contains MOUSEEVENTF_WHEEL, then mouseData specifies the amount of wheel movement. A positive
    ///     value indicates that the wheel was rotated forward, away from the user; a negative value indicates that
    ///     the wheel was rotated backward, toward the user. One wheel click is defined as WHEEL_DELTA, which is
    ///     120.
    /// </summary>
    WHEEL_DELTA = 160,
}
