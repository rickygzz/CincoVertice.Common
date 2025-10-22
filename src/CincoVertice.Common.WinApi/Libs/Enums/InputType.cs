namespace CincoVertice.Common.WinApi.Libs.Enums;

/// <summary>
///     The type of the input event. This member can be one of the following values.
/// </summary>
public enum InputType : uint
{
    /// <summary>The information about a simulated mouse event.</summary>
    INPUT_MOUSE = 0,

    /// <summary>The information about a simulated keyboard event.</summary>
    INPUT_KEYBOARD = 1,

    /// <summary>The information about a simulated hardware event.</summary>
    INPUT_HARDWARE = 2,
}
