using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs.Structs;

// StructLayoutAttribute fields
// .Pack  Controls the alignment of data fields of a class or structure in memory.
// .Size  Indicates the absolute size of the class or structure.
//     This field must be equal or greater than the total size, in bytes, of the members of the class or structure.
//     This field is primarily for compiler writers who want to extend the memory occupied by a structure for direct, unmanaged access.

/// <summary>
///     Contains message information from a thread's message queue.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct WINMSG
{
    /// <summary>
    ///     A handle to the window whose window procedure receives the message. This member is NULL when the message
    ///     is a thread message.
    /// </summary>
    public nint Hwnd;

    /// <summary>
    ///     The message identifier. Applications can only use the low word; the high word is reserved by the system.
    /// </summary>
    public uint Message;

    /// <summary>
    ///     Additional information about the message. The exact meaning depends on the value of the message member.
    /// </summary>
    public nint WParam;

    /// <summary>
    ///     Additional information about the message. The exact meaning depends on the value of the message member.
    /// </summary>
    public nint LParam;

    /// <summary>The time at which the message was posted.</summary>
    public uint Time;

    /// <summary>The cursor position, in screen coordinates, when the message was posted.</summary>
    public int X;

    /// <summary>The cursor position, in screen coordinates, when the message was posted.</summary>
    public int Y;
}
