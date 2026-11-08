using CincoVertice.Common.WinApi.Libs.Enums;
using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs.Structs;

/// <summary>
///     Each structure represents an event to be inserted into the keyboard or mouse input stream.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct INPUT
{
    /// <summary>The type of the input event.</summary>
    public InputType Type;

    /// <summary>Mouse, keyboard or hardware input</summary>
    public MOUSEKEYBDHARDWAREINPUT MKHInput;
}
