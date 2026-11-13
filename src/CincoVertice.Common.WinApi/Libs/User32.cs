using CincoVertice.Common.WinApi.Libs.Enums;
using CincoVertice.Common.WinApi.Libs.Structs;
using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;
public partial class User32
{
    /// <summary>The handle to send a message to all top-level windows.</summary>
    public const nint HWND_BROADCAST = 0xffff;

    /// <summary>
    ///     Sends the message and returns immediately, without waiting for the window to process it, so a window that
    ///     doesn't respond can't block the caller.
    /// </summary>
    /// <param name="hWnd">A handle to the window, or <see cref="HWND_BROADCAST"/> for all top-level windows.</param>
    /// <param name="msg">The message.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">Additional message-specific information.</param>
    /// <returns>Nonzero if the function succeeds.</returns>
    [LibraryImport("user32.dll", EntryPoint = "SendNotifyMessageW", SetLastError = true)]
    public static partial int SendNotifyMessage(nint hWnd, uint msg, nint wParam, nint lParam);

    /// <summary>
    ///     Contains information about the placement of a window on the screen.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        /// <summary>
        ///     The length of the structure, in bytes. Before calling the GetWindowPlacement or SetWindowPlacement
        ///     functions, set this member to sizeof(WINDOWPLACEMENT). GetWindowPlacement and SetWindowPlacement
        ///     fail if this member is not set correctly.
        /// </summary>
        public uint Length;

        /// <summary>
        ///     The flags that control the position of the minimized window and the method by which the window is
        ///     restored. This member can be one or more of the following values.
        /// </summary>
        public uint Flags;

        /// <summary>The current show state of the window. This member can be one of the following values.</summary>
        public ShowCmd ShowCmd;

        /// <summary>The coordinates of the window's upper-left corner when the window is minimized.</summary>
        public POINT PtMinPosition;

        /// <summary>The coordinates of the window's upper-left corner when the window is maximized.</summary>
        public POINT PtMaxPosition;

        /// <summary>The window's coordinates when the window is in the restored position.</summary>
        public RECT RcNormalPosition;

        /// <summary>rcDevice.</summary>
        public RECT RcDevice;
    }

    /// <summary>
    /// Retrieves the show state and the restored, minimized, and maximized positions of the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="lpwndpl">
    ///     A pointer to the WINDOWPLACEMENT structure that receives the show state and position information. Before
    ///     calling GetWindowPlacement, set the length member to sizeof(WINDOWPLACEMENT). GetWindowPlacement fails
    ///     if lpwndpl-> length is not set correctly.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
    ///     To get extended error information, call GetLastError.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetWindowPlacement(nint hWnd, ref WINDOWPLACEMENT lpwndpl);

    /// <summary>
    ///     Sets the show state and the restored, minimized, and maximized positions of the specified window.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="lpwndpl">
    ///     A pointer to a WINDOWPLACEMENT structure that specifies the new show state and window positions.
    ///     <para>
    ///         Before calling SetWindowPlacement, set the length member of the WINDOWPLACEMENT structure to
    ///         sizeof(WINDOWPLACEMENT). SetWindowPlacement fails if the length member is not set correctly.
    ///     </para>
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
    ///     To get extended error information, call GetLastError.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPlacement(nint hWnd, ref WINDOWPLACEMENT lpwndpl);

    /// <summary>
    ///     Retrieves a handle to the top-level window whose class name and window name match the specified strings.
    ///     This function does not search child windows. This function does not perform a case-sensitive search.
    ///     <para>
    ///         To search child windows, beginning with a specified child window, use the FindWindowEx function.
    ///     </para>
    /// </summary>
    /// <param name="lpClassName">
    ///     The class name or a class atom created by a previous call to the RegisterClass or RegisterClassEx
    ///     function. The atom must be in the low-order word of lpClassName; the high-order word must be zero.
    ///     <para>
    ///         If lpClassName points to a string, it specifies the window class name. The class name can be any
    ///         name registered with RegisterClass or RegisterClassEx, or any of the predefined control-class names.
    ///     </para>
    ///     <para>
    ///         If lpClassName is NULL, it finds any window whose title matches the lpWindowName parameter.
    ///     </para>
    /// </param>
    /// <param name="lpWindowName">
    ///     The window name (the window's title). If this parameter is NULL, all window names match.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the window that has the specified class name
    ///     and window name.
    ///     <para>
    ///         If the function fails, the return value is NULL. To get extended error information, call
    ///         GetLastError.
    ///     </para>
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern nint FindWindow(string lpClassName, string lpWindowName);

    /// <summary>
    ///     Retrieves the extra message information for the current thread. Extra message information is an
    ///     application- or driver-defined value associated with the current thread's message queue.
    /// </summary>
    /// <returns>
    ///     The return value specifies the extra information. The meaning of the extra information is device
    ///     specific.
    /// </returns>
    [LibraryImport("user32.dll")]
    public static partial nint GetMessageExtraInfo();

    /// <summary>
    /// Retrieves the cursor position for the last message retrieved by the GetMessage function.
    /// </summary>
    /// <returns>
    ///     The return value specifies the x- and y-coordinates of the cursor position. The x-coordinate is the low
    ///     order short and the y-coordinate is the high-order short
    /// </returns>
    [LibraryImport("user32.dll")]
    public static partial uint GetMessagePos();

    /// <summary>
    ///     Retrieves the message time for the last message retrieved by the GetMessage function. The time is a long
    ///     integer that specifies the elapsed time, in milliseconds, from the time the system was started to the
    ///     time the message was created (that is, placed in the thread's message queue).
    /// </summary>
    /// <returns>The return value specifies the message time.</returns>
    [LibraryImport("user32.dll")]
    public static partial int GetMessageTime();

    /// <summary>
    ///     Retrieves a handle to the desktop window. The desktop window covers the entire screen. The desktop
    ///     window is the area on top of which other windows are painted.
    /// </summary>
    /// <returns>The return value is a handle to the desktop window.</returns>
    [LibraryImport("user32.dll")]
    public static partial nint GetDesktopWindow();

    /// <summary>
    ///     Retrieves a handle to the specified window's parent or owner. To retrieve a handle to a specified
    ///     ancestor, use the GetAncestor function.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose parent window handle is to be retrieved.</param>
    /// <returns>
    ///     If the window is a child window, the return value is a handle to the parent window. If the window is a
    ///     top-level window with the WS_POPUP style, the return value is a handle to the owner window.
    ///     <para>
    ///         If the function fails, the return value is NULL. To get extended error information, call
    ///         GetLastError.
    ///     </para>
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint GetParent(nint hWnd);

    /// <summary>
    /// GetWindowCmd for GetWindow.
    /// </summary>
    public enum GetWindowCmd : uint
    {
        /// <summary>
        ///     The retrieved handle identifies the window of the same type that is highest in the Z order.
        ///     <para>If the specified window is a topmost window, the handle identifies a topmost window.</para>
        ///     <para>If the specified window is a top-level window, the handle identifies a top-level window.</para>
        ///     <para>If the specified window is a child window, the handle identifies a sibling window.</para>
        /// </summary>
        GW_HWNDFIRST = 0,

        /// <summary>
        ///     The retrieved handle identifies the window of the same type that is lowest in the Z order.
        ///     <para>If the specified window is a topmost window, the handle identifies a topmost window.</para>
        ///     <para>If the specified window is a top-level window, the handle identifies a top-level window.</para>
        ///     <para>If the specified window is a child window, the handle identifies a sibling window.</para>
        /// </summary>
        GW_HWNDLAST = 1,

        /// <summary>
        ///     The retrieved handle identifies the window below the specified window in the Z order.
        ///     If the specified window is a topmost window, the handle identifies a topmost window. If the
        ///     specified window is a top-level window, the handle identifies a top-level window. If the specified
        ///     window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDNEXT = 2,

        /// <summary>
        ///     The retrieved handle identifies the window above the specified window in the Z order.
        ///     If the specified window is a topmost window, the handle identifies a topmost window. If the
        ///     specified window is a top-level window, the handle identifies a top-level window. If the specified
        ///     window is a child window, the handle identifies a sibling window.
        /// </summary>
        GW_HWNDPREV = 3,

        /// <summary>
        ///     The retrieved handle identifies the specified window's owner window, if any. For more information,
        ///     see Owned Windows.
        /// </summary>
        GW_OWNER = 4,

        /// <summary>
        ///     The retrieved handle identifies the child window at the top of the Z order, if the specified window
        ///     is a parent window; otherwise, the retrieved handle is NULL. The function examines only child
        ///     windows of the specified window. It does not examine descendant windows.
        /// </summary>
        GW_CHILD = 5,

        /// <summary>
        ///     The retrieved handle identifies the enabled popup window owned by the specified window (the search
        ///     uses the first such window found using GW_HWNDNEXT); otherwise, if there are no enabled popup
        ///     windows, the retrieved handle is that of the specified window.
        /// </summary>
        GW_ENABLEDPOPUP = 6,
    }

    /// <summary>
    ///     Retrieves a handle to a window that has the specified relationship (Z-Order or owner) to the specified
    ///     window.
    /// </summary>
    /// <param name="hWnd">
    ///     A handle to a window. The window handle retrieved is relative to this window, based on the value of the
    ///     uCmd parameter.
    /// </param>
    /// <param name="uCmd">
    ///     The relationship between the specified window and the window whose handle is to be retrieved. This
    ///     parameter can be one of the following values.
    /// </param>
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial nint GetWindow(nint hWnd, GetWindowCmd uCmd);

    /// <summary>
    ///     The GetWindowDC function retrieves the device context (DC) for the entire window, including title bar,
    ///     menus, and scroll bars. A window device context permits painting anywhere in a window, because the
    ///     origin of the device context is the upper-left corner of the window instead of the client area.
    ///     <para>
    ///         GetWindowDC assigns default attributes to the window device context each time it retrieves the
    ///         device context. Previous attributes are lost.
    ///     </para>
    /// </summary>
    /// <param name="hWnd">
    ///     A handle to the window with a device context that is to be retrieved. If this value is NULL, GetWindowDC
    ///     retrieves the device context for the entire screen.
    ///     <para>
    ///         If this parameter is NULL, GetWindowDC retrieves the device context for the primary display monitor.
    ///         To get the device context for other display monitors, use the EnumDisplayMonitors and CreateDC
    ///         functions.
    ///     </para>
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to a device context for the specified window.
    ///     <para>
    ///         If the function fails, the return value is NULL, indicating an error or an invalid hWnd parameter.
    ///     </para>
    /// </returns>
    [LibraryImport("user32.dll")]
    public static partial nint GetWindowDC(nint hWnd);

    /// <summary>
    ///     The ReleaseDC function releases a device context (DC), freeing it for use by other applications. The
    ///     effect of the ReleaseDC function depends on the type of DC. It frees only common and window DCs. It has
    ///     no effect on class or private DCs.
    /// </summary>
    /// <param name="hWnd">A handle to the window whose DC is to be released.</param>
    /// <param name="hDC">A handle to the DC to be released.</param>
    /// <returns>
    ///     The return value indicates whether the DC was released. If the DC was released, the return value is 1.
    ///     <para>If the DC was not released, the return value is zero.</para>
    /// </returns>
    [LibraryImport("user32.dll")]
    public static partial int ReleaseDC(nint hWnd, nint hDC);

    /// <summary>
    ///     Retrieves the dimensions of the bounding rectangle of the specified window. The dimensions are given in
    ///     screen coordinates that are relative to the upper-left corner of the screen.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="lpRect">
    ///     A pointer to a RECT structure that receives the screen coordinates of the upper-left and lower-right
    ///     corners of the window.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.
    ///     <para>
    ///         If the function fails, the return value is zero. To get extended error information, call
    ///         GetLastError.
    ///     </para>
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int GetWindowRect(nint hWnd, out RECT lpRect);

    /// <summary>
    /// Retrieves a handle to the window that contains the specified point.
    /// </summary>
    /// <param name="point">The point to be checked.</param>
    /// <returns>
    ///     The return value is a handle to the window that contains the point. If no window exists at the given
    ///     point, the return value is NULL. If the point is over a static text control, the return value is a
    ///     handle to the window under the static text control.
    /// </returns>
    [LibraryImport("user32.dll")]
    public static partial nint WindowFromPoint(POINT point);

    /// <summary>
    ///     Determines which, if any, of the child windows belonging to a parent window contains the specified
    ///     point. The search is restricted to immediate child windows. Grandchildren, and deeper descendant windows
    ///     are not searched.
    ///     <para>To skip certain child windows, use the ChildWindowFromPointEx function.</para>
    /// </summary>
    /// <param name="hWndParent">A handle to the parent window.</param>
    /// <param name="pt">
    ///     A structure that defines the client coordinates, relative to hWndParent, of the point to be checked.
    /// </param>
    /// <returns>
    ///     The return value is a handle to the child window that contains the point, even if the child window is
    ///     hidden or disabled. If the point lies outside the parent window, the return value is NULL. If the point
    ///     is within the parent window but not within any child window, the return value is a handle to the parent
    ///     window.
    /// </returns>
    [LibraryImport("user32.dll")]
    public static partial nint ChildWindowFromPoint(nint hWndParent, POINT pt);

    /// <summary>
    ///     Determines whether a key is up or down at the time the function is called, and whether the key was
    ///     pressed after a previous call to GetAsyncKeyState.
    /// </summary>
    /// <param name="vKey">
    ///     The virtual-key code. For more information, see Virtual Key Codes. You can use left- and
    ///     right-distinguishing constants to specify certain keys. See the Remarks section for further information.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value specifies whether the key was pressed since the last call to
    ///     GetAsyncKeyState, and whether the key is currently up or down. If the most significant bit is set, the
    ///     key is down, and if the least significant bit is set, the key was pressed after the previous call to
    ///     GetAsyncKeyState. However, you should not rely on this last behavior; for more information, see the
    ///     Remarks.
    ///     <para>The return value is zero for the following cases:</para>
    ///     <para>
    ///         - The current desktop is not the active desktop
    ///     </para>
    ///     <para>
    ///         - The foreground thread belongs to another process and the desktop does not allow the hook or the
    ///         journal record.
    ///     </para>
    /// </returns>
    [LibraryImport("user32.dll")]
    public static partial short GetAsyncKeyState(KeyCode vKey);

    /// <summary>
    ///     Translates a character to the virtual-key code and shift state that type it on the current keyboard
    ///     layout.
    /// </summary>
    /// <param name="ch">The character to translate.</param>
    /// <returns>
    ///     The low byte is the virtual-key code and the high byte the shift state: 1 Shift, 2 Ctrl, 4 Alt (Ctrl+Alt
    ///     is AltGr). -1 when no key on the layout types the character.
    /// </returns>
    [LibraryImport("user32.dll", EntryPoint = "VkKeyScanW")]
    public static partial short VkKeyScan(ushort ch);

    /// <summary>
    /// Retrieves the position of the mouse cursor, in screen coordinates.
    /// <para>BOOL GetCursorPos(LPPOINT lpPoint)</para>
    /// </summary>
    /// <param name="lpPoint">
    ///     A pointer to a POINT structure that receives the screen coordinates of the cursor.
    /// </param>
    /// <returns>
    ///     Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int GetCursorPos(out POINT lpPoint);

    /// <summary>
    ///     Moves the cursor to the specified screen coordinates. If the new coordinates are not within the screen
    ///     rectangle set by the most recent ClipCursor function call, the system automatically adjusts the
    ///     coordinates so that the cursor stays within the rectangle.
    /// </summary>
    /// <param name="x">The new x-coordinate of the cursor, in screen coordinates.</param>
    /// <param name="y">The new y-coordinate of the cursor, in screen coordinates.</param>
    /// <returns>
    ///     Returns nonzero if successful or zero otherwise. To get extended error information, call GetLastError.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int SetCursorPos(int x, int y);

    /// <summary>
    ///     Brings the thread that created the specified window into the foreground and activates the window.
    ///     Keyboard input is directed to the window, and various visual cues are changed for the user. The system
    ///     assigns a slightly higher priority to the thread that created the foreground window than it does to
    ///     other threads.
    ///     <para>
    ///         The system restricts which processes can set the foreground window. A process can set the foreground
    ///         window only if one of the following conditions is true:
    ///     </para>
    ///     <para>
    ///         An application cannot force a window to the foreground while the user is working with another
    ///         window. Instead, Windows flashes the taskbar button of the window to notify the user.
    ///     </para>
    ///     <para>
    ///         A process that can set the foreground window can enable another process to set the foreground window
    ///         by calling the AllowSetForegroundWindow function. The process specified by dwProcessId loses the
    ///         ability to set the foreground window the next time the user generates input, unless the input is
    ///         directed at that process, or the next time a process calls AllowSetForegroundWindow, unless that
    ///         process is specified.
    ///     </para>
    /// </summary>
    /// <param name="hWnd">
    ///     A handle to the window that should be activated and brought to the foreground.
    /// </param>
    /// <returns>
    ///     If the window was brought to the foreground, the return value is nonzero. If the window was not brought
    ///     to the foreground, the return value is zero.
    /// </returns>
    [LibraryImport("User32.dll")]
    public static partial int SetForegroundWindow(nint hWnd);

    /// <summary>
    ///     Says where a window may be shown: anywhere, or only on the monitors, leaving it out of screenshots and
    ///     screen recordings.
    /// </summary>
    /// <param name="hWnd">A top-level window created by the calling process.</param>
    /// <param name="dwAffinity">Where it may be shown.</param>
    /// <returns>True when it worked; WDA_EXCLUDEFROMCAPTURE fails before Windows 10, version 2004.</returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowDisplayAffinity(nint hWnd, DisplayAffinity dwAffinity);

    /// <summary>Where a window may be shown, for <see cref="SetWindowDisplayAffinity"/>.</summary>
    public enum DisplayAffinity : uint
    {
        /// <summary>Anywhere.</summary>
        WDA_NONE = 0x00,

        /// <summary>Only on the monitors: in a capture, what's behind it shows instead.</summary>
        WDA_EXCLUDEFROMCAPTURE = 0x11,
    }

    /// <summary>
    /// Synthesizes keystrokes, mouse motions, and button clicks.
    /// </summary>
    /// <param name="cInputs">The number of structures in the pInputs array.</param>
    /// <param name="pInputs">
    ///     An array of INPUT structures. Each structure represents an event to be inserted into the keyboard or
    ///     mouse input stream.
    /// </param>
    /// <param name="cbSize">
    ///     The size, in bytes, of an INPUT structure. If cbSize is not the size of an INPUT structure, the function
    ///     fails.
    /// </param>
    /// <returns>
    ///     The function returns the number of events that it successfully inserted into the keyboard or mouse input
    ///     stream. If the function returns zero, the input was already blocked by another thread. To get extended
    ///     error information, call GetLastError.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial uint SendInput(uint cInputs, INPUT[] pInputs, int cbSize);

    /// <summary>
    ///     The MapWindowPoints function converts (maps) a set of points from a coordinate space relative to one
    ///     window to a coordinate space relative to another window.
    /// </summary>
    /// <param name="hWndFrom">
    ///     A handle to the window from which points are converted. If this parameter is NULL or HWND_DESKTOP, the
    ///     points are presumed to be in screen coordinates.
    /// </param>
    /// <param name="hWndTo">
    ///     A handle to the window to which points are converted. If this parameter is NULL or HWND_DESKTOP, the
    ///     points are converted to screen coordinates.
    /// </param>
    /// <param name="lpPoints">
    ///     A pointer to an array of POINT structures that contain the set of points to be converted. The points are
    ///     in device units. This parameter can also point to a RECT structure, in which case the cPoints parameter
    ///     should be set to 2.
    /// </param>
    /// <param name="cPoints">
    ///     The number of POINT structures in the array pointed to by the lpPoints parameter.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the low-order word of the return value is the number of pixels added to the
    ///     horizontal coordinate of each source point in order to compute the horizontal coordinate of each
    ///     destination point. (In addition to that, if precisely one of hWndFrom and hWndTo is mirrored, then each
    ///     resulting horizontal coordinate is multiplied by -1.) The high-order word is the number of pixels added
    ///     to the vertical coordinate of each source point in order to compute the vertical coordinate of each
    ///     destination point.
    ///     <para>
    ///         If the function fails, the return value is zero.Call SetLastError prior to calling this method to
    ///         differentiate an error return value from a legitimate "0" return value.
    ///     </para>
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int MapWindowPoints(nint hWndFrom, nint hWndTo, ref POINT lpPoints, uint cPoints);

    /// <summary>
    ///     The PrintWindow function copies a visual window into the specified device context (DC), typically a
    ///     printer DC.
    /// </summary>
    /// <param name="hwnd">A handle to the window that will be copied.</param>
    /// <param name="hdcBlt">A handle to the device context.</param>
    /// <param name="nFlags">
    ///     The drawing options. It can be one of the following values. PW_CLIENTONLY Only the client area of the
    ///     window is copied to hdcBlt.By default, the entire window is copied.
    /// </param>
    /// <returns>
    ///     If the function succeeds, it returns a nonzero value. If the function fails, it returns zero.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool PrintWindow(nint hwnd, nint hdcBlt, uint nFlags);

    /// <summary>
    /// Defines a system-wide hot key.
    /// </summary>
    /// <param name="hWnd">
    ///     A handle to the window that will receive WM_HOTKEY messages generated by the hot key. If this parameter
    ///     is NULL, WM_HOTKEY messages are posted to the message queue of the calling thread and must be processed
    ///     in the message loop.
    /// </param>
    /// <param name="id">
    ///     The identifier of the hot key. If the hWnd parameter is NULL, then the hot key is associated with the
    ///     current thread rather than with a particular window. If a hot key already exists with the same hWnd and
    ///     id parameters, see Remarks for the action taken.
    /// </param>
    /// <param name="fsModifiers">
    ///     The keys that must be pressed in combination with the key specified by the uVirtKey parameter in order
    ///     to generate the WM_HOTKEY message. The fsModifiers parameter can be a combination of the following
    ///     values.
    /// </param>
    /// <param name="vk">The virtual-key code of the hot key. See Virtual Key Codes.</param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
    ///     To get extended error information, call GetLastError.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int RegisterHotKey(nint hWnd, int id, FSModifiers fsModifiers, KeyCode vk);

    /// <summary>
    /// Frees a hot key previously registered by the calling thread.
    /// </summary>
    /// <param name="hWnd">
    ///     A handle to the window associated with the hot key to be freed. This parameter should be NULL if the hot
    ///     key is not associated with a window.
    /// </param>
    /// <param name="id">The identifier of the hot key to be freed.</param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
    ///     To get extended error information, call GetLastError.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    public static partial int UnregisterHotKey(nint hWnd, int id);

    /// <summary>
    ///     The DPI awareness context in which coordinates are physical pixels on every monitor, even when the
    ///     monitors have different scaling; windows created in it get WM_DPICHANGED when they move between monitors.
    /// </summary>
    public const nint DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = -4;

    /// <summary>
    ///     Sets the DPI awareness of the calling thread. Windows created afterwards keep the awareness they were
    ///     created with, and their window procedure runs in it.
    /// </summary>
    /// <param name="dpiContext">The new DPI awareness context, e.g. DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2.</param>
    /// <returns>The previous DPI awareness context, to restore it; zero when dpiContext is not valid.</returns>
    [LibraryImport("user32.dll")]
    public static partial nint SetThreadDpiAwarenessContext(nint dpiContext);

    /// <summary>Sent when the user starts moving or resizing a window.</summary>
    public const uint EVENT_SYSTEM_MOVESIZESTART = 0x000A;

    /// <summary>Sent when the user ends moving or resizing a window.</summary>
    public const uint EVENT_SYSTEM_MOVESIZEEND = 0x000B;

    /// <summary>The hook function is called in the hooking thread, from its message loop: no DLL is injected.</summary>
    public const uint WINEVENT_OUTOFCONTEXT = 0x0000;

    /// <summary>Events of the hooking process's own windows aren't sent.</summary>
    public const uint WINEVENT_SKIPOWNPROCESS = 0x0002;

    /// <summary>The object of an event that is the window itself, not a part of it.</summary>
    public const int OBJID_WINDOW = 0;

    /// <summary>The window, not one of its children, for GetAncestor.</summary>
    public const uint GA_ROOT = 2;

    /// <summary>The monitor nearest the window, when it's on none.</summary>
    public const uint MONITOR_DEFAULTTONEAREST = 2;

    /// <summary>A function Windows calls for a window event (WinEventProc).</summary>
    /// <param name="hWinEventHook">The hook.</param>
    /// <param name="eventType">Which event, e.g. EVENT_SYSTEM_MOVESIZESTART.</param>
    /// <param name="hwnd">The window of the event.</param>
    /// <param name="idObject">The object of the event, e.g. OBJID_WINDOW.</param>
    /// <param name="idChild">0 for the object itself.</param>
    /// <param name="idEventThread">The thread of the event.</param>
    /// <param name="dwmsEventTime">When it happened, in milliseconds.</param>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate void WinEventProc(
        nint hWinEventHook,
        uint eventType,
        nint hwnd,
        int idObject,
        int idChild,
        uint idEventThread,
        uint dwmsEventTime);

    /// <summary>A function Windows calls for each monitor (MonitorEnumProc).</summary>
    /// <returns>True to go on to the next monitor.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public delegate bool MonitorEnumProc(nint hMonitor, nint hdcMonitor, nint lprcMonitor, nint dwData);

    /// <summary>How SetWindowPos changes a window.</summary>
    [Flags]
    public enum SetWindowPosFlags : uint
    {
        /// <summary>Keeps the size.</summary>
        SWP_NOSIZE = 0x0001,

        /// <summary>Keeps the position.</summary>
        SWP_NOMOVE = 0x0002,

        /// <summary>Keeps the Z order.</summary>
        SWP_NOZORDER = 0x0004,

        /// <summary>Doesn't activate the window.</summary>
        SWP_NOACTIVATE = 0x0010,

        /// <summary>Keeps the Z order of the owner window.</summary>
        SWP_NOOWNERZORDER = 0x0200,
    }

    /// <summary>A monitor's bounds and work area (MONITORINFO).</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MONITORINFO
    {
        /// <summary>The size of the structure, set before the call.</summary>
        public int cbSize;

        /// <summary>The whole monitor.</summary>
        public RECT rcMonitor;

        /// <summary>The monitor without the taskbar and docked toolbars.</summary>
        public RECT rcWork;

        /// <summary>1 for the primary monitor.</summary>
        public uint dwFlags;
    }

    /// <summary>
    ///     Calls a function for a range of window events in every process. With WINEVENT_OUTOFCONTEXT the calls come
    ///     through the message loop of the calling thread, which must have one.
    /// </summary>
    /// <param name="eventMin">The first event, e.g. EVENT_SYSTEM_MOVESIZESTART.</param>
    /// <param name="eventMax">The last event.</param>
    /// <param name="hmodWinEventProc">0 with WINEVENT_OUTOFCONTEXT.</param>
    /// <param name="lpfnWinEventProc">
    ///     A pointer to a <see cref="WinEventProc"/>, which the caller keeps alive until the hook is removed.
    /// </param>
    /// <param name="idProcess">0 for every process.</param>
    /// <param name="idThread">0 for every thread.</param>
    /// <param name="dwFlags">E.g. WINEVENT_OUTOFCONTEXT | WINEVENT_SKIPOWNPROCESS.</param>
    /// <returns>The hook, for UnhookWinEvent; 0 when it failed.</returns>
    [LibraryImport("user32.dll")]
    public static partial nint SetWinEventHook(
        uint eventMin,
        uint eventMax,
        nint hmodWinEventProc,
        nint lpfnWinEventProc,
        uint idProcess,
        uint idThread,
        uint dwFlags);

    /// <summary>Removes a hook made by SetWinEventHook.</summary>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnhookWinEvent(nint hWinEventHook);

    /// <summary>Changes the size, position and Z order of a window.</summary>
    /// <param name="hWnd">The window.</param>
    /// <param name="hWndInsertAfter">The window to put it after in the Z order; 0 with SWP_NOZORDER.</param>
    /// <param name="X">The new left edge.</param>
    /// <param name="Y">The new top edge.</param>
    /// <param name="cx">The new width.</param>
    /// <param name="cy">The new height.</param>
    /// <param name="uFlags">What to keep.</param>
    /// <returns>
    ///     True when it worked; false otherwise, e.g. with ERROR_ACCESS_DENIED for a window of a program that runs as
    ///     administrator, when the caller doesn't.
    /// </returns>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool SetWindowPos(
        nint hWnd,
        nint hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        SetWindowPosFlags uFlags);

    /// <summary>Whether the window is maximized.</summary>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsZoomed(nint hWnd);

    /// <summary>A window's ancestor; with GA_ROOT, its top-level window.</summary>
    [LibraryImport("user32.dll")]
    public static partial nint GetAncestor(nint hwnd, uint gaFlags);

    /// <summary>The monitor a window is on, or with MONITOR_DEFAULTTONEAREST the nearest one.</summary>
    [LibraryImport("user32.dll")]
    public static partial nint MonitorFromWindow(nint hwnd, uint dwFlags);

    /// <summary>A monitor's bounds and work area.</summary>
    /// <param name="hMonitor">The monitor.</param>
    /// <param name="lpmi">Receives them; its cbSize is set before the call.</param>
    /// <returns>True when it worked.</returns>
    [LibraryImport("user32.dll", EntryPoint = "GetMonitorInfoW")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetMonitorInfo(nint hMonitor, ref MONITORINFO lpmi);

    /// <summary>Calls a function for each monitor.</summary>
    /// <param name="hdc">0 for every monitor of the desktop.</param>
    /// <param name="lprcClip">0 for no clipping.</param>
    /// <param name="lpfnEnum">A pointer to a <see cref="MonitorEnumProc"/>, kept alive during the call.</param>
    /// <param name="dwData">Passed to the function.</param>
    /// <returns>True when it worked.</returns>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool EnumDisplayMonitors(nint hdc, nint lprcClip, nint lpfnEnum, nint dwData);

    /// <summary>The window's styles, for GetWindowLongPtr.</summary>
    public const int GWL_STYLE = -16;

    /// <summary>The window's extended styles (WS_EX_*), for GetWindowLongPtr.</summary>
    public const int GWL_EXSTYLE = -20;

    /// <summary>A title bar.</summary>
    public const long WS_CAPTION = 0x00C00000;

    /// <summary>A border to resize the window with.</summary>
    public const long WS_THICKFRAME = 0x00040000;

    /// <summary>A value of the window, e.g. its styles with GWL_STYLE.</summary>
    [LibraryImport("user32.dll", EntryPoint = "GetWindowLongPtrW")]
    public static partial nint GetWindowLongPtr(nint hWnd, int nIndex);

    /// <summary>Whether the window has the visible style.</summary>
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool IsWindowVisible(nint hWnd);

    /// <summary>The desktop's window, behind every other.</summary>
    [LibraryImport("user32.dll")]
    public static partial nint GetShellWindow();

    /// <summary>The cursor is shown, for CURSORINFO.flags.</summary>
    public const uint CURSOR_SHOWING = 0x00000001;

    /// <summary>The icon or cursor drawn with its mask and image, as Windows does, for DrawIconEx.</summary>
    public const uint DI_NORMAL = 0x0003;

    /// <summary>The cursor, and where it is (CURSORINFO).</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CURSORINFO
    {
        /// <summary>The size of the structure, set before the call.</summary>
        public int cbSize;

        /// <summary>CURSOR_SHOWING when it's shown.</summary>
        public uint flags;

        /// <summary>The cursor.</summary>
        public nint hCursor;

        /// <summary>Where it points, in screen coordinates.</summary>
        public POINT ptScreenPos;
    }

    /// <summary>An icon or cursor's hot spot and bitmaps (ICONINFO); the caller deletes the bitmaps.</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ICONINFO
    {
        /// <summary>Nonzero for an icon, 0 for a cursor.</summary>
        public int fIcon;

        /// <summary>The point of the cursor that points, from its left.</summary>
        public int xHotspot;

        /// <summary>The same, from its top.</summary>
        public int yHotspot;

        /// <summary>Its mask bitmap.</summary>
        public nint hbmMask;

        /// <summary>Its color bitmap; 0 for a black and white cursor.</summary>
        public nint hbmColor;
    }

    /// <summary>The cursor shown now, and where, in the DPI awareness of the calling thread.</summary>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetCursorInfo(ref CURSORINFO pci);

    /// <summary>An icon or cursor's hot spot and bitmaps; the caller deletes the bitmaps with DeleteObject.</summary>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool GetIconInfo(nint hIcon, out ICONINFO piconinfo);

    /// <summary>Draws an icon or cursor; a width and height of 0 draw it at its usual size.</summary>
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool DrawIconEx(
        nint hdc,
        int xLeft,
        int yTop,
        nint hIcon,
        int cxWidth,
        int cyWidth,
        uint istepIfAniCur,
        nint hbrFlickerFreeDraw,
        uint diFlags);

    /// <summary>A low-level hook of the mouse, for SetWindowsHookEx.</summary>
    public const int WH_MOUSE_LL = 14;

    /// <summary>A low-level hook of the keyboard, for SetWindowsHookEx.</summary>
    public const int WH_KEYBOARD_LL = 13;

    /// <summary>Messages a low-level hook gets, in its wParam.</summary>
    public const int WM_KEYDOWN = 0x0100;
    public const int WM_SYSKEYDOWN = 0x0104;
    public const int WM_LBUTTONDOWN = 0x0201;
    public const int WM_RBUTTONDOWN = 0x0204;
    public const int WM_MBUTTONDOWN = 0x0207;

    /// <summary>The character a key types, for MapVirtualKey; 0 when it types none.</summary>
    public const uint MAPVK_VK_TO_CHAR = 2;

    /// <summary>
    ///     A function Windows calls for each input, for SetWindowsHookEx (LowLevelMouseProc, LowLevelKeyboardProc).
    /// </summary>
    /// <returns>What CallNextHookEx returns, so the input goes on.</returns>
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    public delegate nint LowLevelHookProc(int nCode, nint wParam, nint lParam);

    /// <summary>A mouse input, as a low-level hook gets it (MSLLHOOKSTRUCT).</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct MSLLHOOKSTRUCT
    {
        /// <summary>Where the pointer is, in per-monitor aware screen coordinates: physical pixels.</summary>
        public POINT pt;

        public uint mouseData;

        public uint flags;

        public uint time;

        public nint dwExtraInfo;
    }

    /// <summary>A key, as a low-level hook gets it (KBDLLHOOKSTRUCT).</summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct KBDLLHOOKSTRUCT
    {
        /// <summary>The virtual key code.</summary>
        public uint vkCode;

        public uint scanCode;

        public uint flags;

        public uint time;

        public nint dwExtraInfo;
    }

    /// <summary>
    ///     Calls a function for the input of every program; the calls come through the calling thread's messages.
    /// </summary>
    /// <param name="idHook">WH_MOUSE_LL or WH_KEYBOARD_LL.</param>
    /// <param name="lpfn">A pointer to a <see cref="LowLevelHookProc"/>, kept alive until the hook is removed.</param>
    /// <param name="hmod">The module of the calling program.</param>
    /// <param name="dwThreadId">0 for every thread.</param>
    /// <returns>The hook; 0 when it failed.</returns>
    [LibraryImport("user32.dll", EntryPoint = "SetWindowsHookExW", SetLastError = true)]
    public static partial nint SetWindowsHookEx(int idHook, nint lpfn, nint hmod, uint dwThreadId);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static partial bool UnhookWindowsHookEx(nint hhk);

    /// <summary>Passes the input on to the next hook, so it reaches the program it's for.</summary>
    [LibraryImport("user32.dll")]
    public static partial nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    /// <summary>Stops a window drawing itself (wParam 0) or lets it again (1); invalidate it after.</summary>
    public const uint WM_SETREDRAW = 0x000B;

    /// <summary>The line at the top of an edit or rich edit control, from 0.</summary>
    public const uint EM_GETFIRSTVISIBLELINE = 0x00CE;

    /// <summary>Scrolls an edit or rich edit control by lParam lines: down when positive, up when negative.</summary>
    public const uint EM_LINESCROLL = 0x00B6;

    /// <summary>
    ///     Sends a message to a window and waits until it's handled. Only for windows of this thread, or ones that
    ///     answer: one that doesn't blocks the caller.
    /// </summary>
    /// <returns>What the window answered, which depends on the message.</returns>
    [LibraryImport("user32.dll", EntryPoint = "SendMessageW")]
    public static partial nint SendMessage(nint hWnd, uint msg, nint wParam, nint lParam);

    /// <summary>Translates a virtual key code, e.g. with MAPVK_VK_TO_CHAR to the character it types.</summary>
    [LibraryImport("user32.dll", EntryPoint = "MapVirtualKeyW")]
    public static partial uint MapVirtualKey(uint uCode, uint uMapType);
}
