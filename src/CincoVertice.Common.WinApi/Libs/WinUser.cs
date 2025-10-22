using CincoVertice.Common.WinApi.Libs.Enums;
using CincoVertice.Common.WinApi.Libs.Structs;
using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;

public static class WinUser
{
    /// <summary>
    ///     Brings the specified window to the top of the Z order. If the window is a top-level window, it is activated.
    ///     If the window is a child window, the top-level parent window associated with the child window is activated.
    /// </summary>
    /// <param name="hWnd">A handle to the window to bring to the top of the Z order.</param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.
    ///     <para>
    ///         If the function fails, the return value is zero.To get extended error information, call
    ///         GetLastError.
    ///     </para>
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool BringWindowToTop(nint hWnd);

    /// <summary>
    ///     Creates an overlapped, pop-up, or child window with an extended window style; otherwise, this function is
    ///     identical to the CreateWindow function. For more information about creating a window and for full
    ///     descriptions of the other parameters of CreateWindowEx, see CreateWindow.
    /// </summary>
    /// <param name="dwExStyle">
    ///     The extended window style of the window being created. For a list of possible values, see Extended
    ///     Window Styles.
    /// </param>
    /// <param name="lpClassName">
    ///     A null-terminated string or a class atom created by a previous call to the RegisterClass or
    ///     RegisterClassEx function. The atom must be in the low-order word of lpClassName; the high-order word
    ///     must be zero. If lpClassName is a string, it specifies the window class name. The class name can be any
    ///     name registered with RegisterClass or RegisterClassEx, provided that the module that registers the class
    ///     is also the module that creates the window. The class name can also be any of the predefined system
    ///     class names.
    /// </param>
    /// <param name="lpWindowName">
    ///     The window name. If the window style specifies a title bar, the window title pointed to by lpWindowName
    ///     is displayed in the title bar. When using CreateWindow to create controls, such as buttons, check boxes,
    ///     and static controls, use lpWindowName to specify the text of the control. When creating a static control
    ///     with the SS_ICON style, use lpWindowName to specify the icon name or identifier. To specify an
    ///     identifier, use the syntax "#num".
    /// </param>
    /// <param name="dwStyle">
    ///     The style of the window being created. This parameter can be a combination of the window style values,
    ///     plus the control styles indicated in the Remarks section.
    /// </param>
    /// <param name="x">
    ///     The initial horizontal position of the window. For an overlapped or pop-up window, the x parameter is
    ///     the initial x-coordinate of the window's upper-left corner, in screen coordinates. For a child window,
    ///     x is the x-coordinate of the upper-left corner of the window relative to the upper-left corner of the
    ///     parent window's client area. If x is set to CW_USEDEFAULT, the system selects the default position for
    ///     the window's upper-left corner and ignores the y parameter. CW_USEDEFAULT is valid only for overlapped
    ///     windows; if it is specified for a pop-up or child window, the x and y parameters are set to zero.
    /// </param>
    /// <param name="y">
    ///     The initial vertical position of the window. For an overlapped or pop-up window, the y parameter is the
    ///     initial y-coordinate of the window's upper-left corner, in screen coordinates. For a child window, y is
    ///     the initial y-coordinate of the upper-left corner of the child window relative to the upper-left corner
    ///     of the parent window's client area. For a list box y is the initial y-coordinate of the upper-left
    ///     corner of the list box's client area relative to the upper-left corner of the parent window's client
    ///     area.
    ///     <para>
    ///         If an overlapped window is created with the WS_VISIBLE style bit set and the x parameter is set to
    ///         CW_USEDEFAULT, then the y parameter determines how the window is shown. If the y parameter is
    ///         CW_USEDEFAULT, then the window manager calls ShowWindow with the SW_SHOW flag after the window has
    ///         been created. If the y parameter is some other value, then the window manager calls ShowWindow with
    ///         that value as the nCmdShow parameter.
    ///     </para>
    /// </param>
    /// <param name="nWidth">
    ///     The width, in device units, of the window. For overlapped windows, nWidth is the window's width, in
    ///     screen coordinates, or CW_USEDEFAULT. If nWidth is CW_USEDEFAULT, the system selects a default width and
    ///     height for the window; the default width extends from the initial x-coordinates to the right edge of the
    ///     screen; the default height extends from the initial y-coordinate to the top of the icon area.
    ///     CW_USEDEFAULT is valid only for overlapped windows; if CW_USEDEFAULT is specified for a pop-up or child
    ///     window, the nWidth and nHeight parameter are set to zero.
    /// </param>
    /// <param name="nHeight">
    ///     The height, in device units, of the window. For overlapped windows, nHeight is the window's height, in
    ///     screen coordinates. If the nWidth parameter is set to CW_USEDEFAULT, the system ignores nHeight.
    /// </param>
    /// <param name="hWndParent">
    ///     A handle to the parent or owner window of the window being created. To create a child window or an owned
    ///     window, supply a valid window handle. This parameter is optional for pop-up windows.
    ///     <para>
    ///         To create a message-only window, supply HWND_MESSAGE or a handle to an existing message-only window.
    ///     </para>
    /// </param>
    /// <param name="hMenu">
    ///     A handle to a menu, or specifies a child-window identifier, depending on the window style. For an
    ///     overlapped or pop-up window, hMenu identifies the menu to be used with the window; it can be NULL if the
    ///     class menu is to be used. For a child window, hMenu specifies the child-window identifier, an integer
    ///     value used by a dialog box control to notify its parent about events. The application determines the
    ///     child-window identifier; it must be unique for all child windows with the same parent window.
    /// </param>
    /// <param name="hInstance">A handle to the instance of the module to be associated with the window.</param>
    /// <param name="lpParam">
    ///     Pointer to a value to be passed to the window through the CREATESTRUCT structure (lpCreateParams member)
    ///     pointed to by the lParam param of the WM_CREATE message. This message is sent to the created window by
    ///     this function before it returns.
    ///     <para>
    ///         If an application calls CreateWindow to create a MDI client window, lpParam should point to a
    ///         CLIENTCREATESTRUCT structure.If an MDI client window calls CreateWindow to create an MDI child
    ///         window, lpParam should point to a MDICREATESTRUCT structure.lpParam may be NULL if no additional
    ///         data is needed.
    ///     </para>
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the new window.
    ///     <para>
    ///         If the function fails, the return value is NULL.To get extended error information, call
    ///         GetLastError.
    ///     </para>
    ///     <para>This function typically fails for one of the following reasons:</para>
    ///     <para>- an invalid parameter value</para>
    ///     <para>- the system class was registered by a different module</para>
    ///     <para>- The WH_CBT hook is installed and returns a failure code</para>
    ///     <para>
    ///         - if one of the controls in the dialog template is not registered, or its window window procedure
    ///         fails WM_CREATE or WM_NCCREATE
    ///     </para>
    /// </returns>
    [DllImport("user32.dll", SetLastError = true, EntryPoint = "CreateWindowEx")]
    public static extern IntPtr CreateWindowEx(
        uint dwExStyle,
        [MarshalAs(UnmanagedType.LPStr)]
            string lpClassName,
        [MarshalAs(UnmanagedType.LPStr)]
            string lpWindowName,
        uint dwStyle,
        int x,
        int y,
        int nWidth,
        int nHeight,
        IntPtr hWndParent,
        IntPtr hMenu,
        IntPtr hInstance,
        IntPtr lpParam);

    /// <summary>
    ///     Destroys the specified window. The function sends WM_DESTROY and WM_NCDESTROY messages to the window to
    ///     deactivate it and remove the keyboard focus from it. The function also destroys the window's menu, flushes
    ///     the thread message queue, destroys timers, removes clipboard ownership, and breaks the clipboard viewer
    ///     chain (if the window is at the top of the viewer chain).
    /// <para>
    ///     If the specified window is a parent or owner window, DestroyWindow automatically destroys the associated
    ///     child or owned windows when it destroys the parent or owner window.The function first destroys child or
    ///     owned windows, and then it destroys the parent or owner window.
    /// </para>
    /// <para>
    ///     DestroyWindow also destroys modeless dialog boxes created by the CreateDialog function.
    /// </para>
    /// </summary>
    /// <param name="hWnd">A handle to the window to be destroyed.</param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.
    ///     <para>
    ///         If the function fails, the return value is zero.To get extended error information, call
    ///         GetLastError.
    ///     </para>
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool DestroyWindow([In] IntPtr hWnd);

    /// <summary>
    ///     Calls the default window procedure to provide default processing for any window messages that an application
    ///     does not process. This function ensures that every message is processed. DefWindowProc is called with the
    ///     same parameters received by the window procedure.
    /// </summary>
    /// <param name="hWnd">A handle to the window procedure that received the message.</param>
    /// <param name="uMsg">The message.</param>
    /// <param name="wParam">
    ///     Additional message information. The content of this parameter depends on the value of the Msg parameter.
    /// </param>
    /// <param name="lParam">
    ///     Additional message information. The content of this parameter depends on the value of the Msg parameter.
    /// </param>
    /// <returns>
    ///     The return value is the result of the message processing and depends on the message.
    /// </returns>
    [DllImport("user32.dll")]
    public static extern IntPtr DefWindowProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    ///     Registers a window class for subsequent use in calls to the CreateWindow or CreateWindowEx function.
    /// </summary>
    /// <param name="lpWndClass">
    ///     A pointer to a WNDCLASSEX structure. You must fill the structure with the appropriate class attributes
    ///     before passing it to the function.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a class atom that uniquely identifies the class being
    ///     registered. This atom can only be used by the CreateWindow, CreateWindowEx, GetClassInfo, GetClassInfoEx,
    ///     FindWindow, FindWindowEx, and UnregisterClass functions and the IActiveIMMap::FilterClientWindows method.
    ///     If the function fails, the return value is zero. To get extended error information, call GetLastError.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern ushort RegisterClassEx([In] ref WNDCLASSEX lpWndClass);

    /// <summary>
    ///     Unregisters a window class, freeing the memory required for the class.
    /// </summary>
    /// <param name="lpClassName">
    ///     A null-terminated string or a class atom. If lpClassName is a string, it specifies the window class
    ///     name. This class name must have been registered by a previous call to the RegisterClass or
    ///     RegisterClassEx function. System classes, such as dialog box controls, cannot be unregistered. If this
    ///     parameter is an atom, it must be a class atom created by a previous call to the RegisterClass or
    ///     RegisterClassEx function. The atom must be in the low-order word of lpClassName; the high-order word
    ///     must be zero.
    /// </param>
    /// <param name="hInstance">A handle to the instance of the module that created the class.</param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero. If the class could not be found or if a window
    ///     still exists that was created with the class, the return value is zero.To get extended error
    ///     information, call GetLastError.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool UnregisterClass([In] string lpClassName, [In, Optional] IntPtr hInstance);

    /// <summary>
    ///     Retrieves the name of the class to which the specified window belongs.
    /// </summary>
    /// <param name="hWnd">A handle to the window and, indirectly, the class to which the window belongs.</param>
    /// <param name="lpClassName">The class name string.</param>
    /// <param name="nMaxCount">
    ///     The length of the lpClassName buffer, in characters. The buffer must be large enough to include the
    ///     terminating null character; otherwise, the class name string is truncated to nMaxCount-1 characters.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is the number of characters copied to the buffer, not
    ///     including the terminating null character.
    ///     <para>
    ///         If the function fails, the return value is zero. To get extended error information, call
    ///         GetLastError function.
    ///     </para>
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetClassName(nint hWnd, System.Text.StringBuilder lpClassName, int nMaxCount);

    /// <summary>
    ///     Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window
    ///     is a control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a
    ///     control in another application.
    /// </summary>
    /// <param name="hWnd">A handle to the window or control containing the text.</param>
    /// <param name="lpString">
    ///     The buffer that will receive the text. If the string is as long or longer than the buffer, the string is
    ///     truncated and terminated with a null character.
    /// </param>
    /// <param name="nMaxCount">
    ///     The maximum number of characters to copy to the buffer, including the null character. If the text
    ///     exceeds this limit, it is truncated.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is the length, in characters, of the copied string, not
    ///     including the terminating null character. If the window has no title bar or text, if the title bar is
    ///     empty, or if the window or control handle is invalid, the return value is zero. To get extended error
    ///     information, call GetLastError.
    ///     <para>This function cannot retrieve the text of an edit control in another application.</para>
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowText(nint hWnd, System.Text.StringBuilder lpString, int nMaxCount);

    /// <summary>
    ///     Retrieves a handle to the foreground window (the window with which the user is currently working). The
    ///     system assigns a slightly higher priority to the thread that creates the foreground window than it does to
    ///     other threads.
    /// </summary>
    /// <returns>
    ///     The return value is a handle to the foreground window. The foreground window can be NULL in certain
    ///     circumstances, such as when a window is losing activation.
    /// </returns>
    [DllImport("user32.dll")]
    public static extern nint GetForegroundWindow();

    /// <summary>
    ///     Retrieves the window handle to the active window attached to the calling thread's message queue.
    /// </summary>
    /// <returns>
    ///     The return value is the handle to the active window attached to the calling thread's message queue.
    ///     Otherwise, the return value is NULL.
    /// </returns>
    [DllImport("user32.dll")]
    public static extern nint GetActiveWindow();

    /// <summary>
    ///     Determines whether the specified window is minimized (iconic).
    /// </summary>
    /// <param name="hWnd">A handle to the window to be tested.</param>
    /// <returns>
    ///     If the window is iconic, the return value is nonzero. If the window is not iconic, the return value is
    ///     zero.
    /// </returns>
    [DllImport("user32.dll")]
    public static extern bool IsIconic(IntPtr hWnd);

    /// <summary>
    ///     Activates a window. The window must be attached to the calling thread's message queue.
    ///     <para>Check SetForegroundWindow.</para>
    ///     <para>
    ///         The SetActiveWindow function activates a window, but not if the application is in the background.
    ///         The window will be brought into the foreground (top of Z-Order) if its application is in the foreground
    ///         when the system activates the window.
    ///     </para>
    ///     <para>
    ///         If the window identified by the hWnd parameter was created by the calling thread, the active window
    ///         status of the calling thread is set to hWnd. Otherwise, the active window status of the calling thread
    ///         is set to NULL.
    ///     </para>
    /// </summary>
    /// <param name="hWnd">A handle to the top-level window to be activated.</param>
    /// <returns>
    ///     If the function succeeds, the return value is the handle to the window that was previously active.
    ///     If the function fails, the return value is NULL. To get extended error information, call GetLastError.
    /// </returns>
    [DllImport("user32.dll")]
    public static extern nint SetActiveWindow(IntPtr hWnd);

    /// <summary>
    ///     Sends the specified message to a window or windows. The SendMessage function calls the window procedure for
    ///     the specified window and does not return until the window procedure has processed the message.
    ///     <para>
    ///         To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To
    ///         post a message to a thread's message queue and return immediately, use the PostMessage or
    ///         PostThreadMessage function.
    ///     </para>
    /// </summary>
    /// <param name="hWnd">
    ///     A handle to the window whose window procedure will receive the message. If this parameter is
    ///     HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including
    ///     disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not
    ///     sent to child windows.
    ///     <para>
    ///         Message sending is subject to UIPI. The thread of a process can send messages only to message queues
    ///         of threads in processes of lesser or equal integrity level.
    ///     </para>
    /// </param>
    /// <param name="wMsg">The message to be sent.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">lParam Additional message-specific information.</param>
    /// <returns>
    ///     The return value specifies the result of the message processing; it depends on the message sent.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int SendMessage(nint hWnd, WM wMsg, nint wParam, nint lParam);

    /// <summary>
    ///     Sends the specified message to a window or windows. The SendMessage function calls the window procedure for
    ///     the specified window and does not return until the window procedure has processed the message.
    /// <para>
    ///     To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To
    ///     post a message to a thread's message queue and return immediately, use the PostMessage or
    ///     PostThreadMessage function.
    /// </para>
    /// </summary>
    /// <param name="hWnd">
    ///     A handle to the window whose window procedure will receive the message. If this parameter is
    ///     HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including
    ///     disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not
    ///     sent to child windows.
    ///     <para>
    ///         Message sending is subject to UIPI. The thread of a process can send messages only to message queues
    ///         of threads in processes of lesser or equal integrity level.
    ///     </para>
    /// </param>
    /// <param name="wMsg">The message to be sent.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">lParam Additional message-specific information.</param>
    /// <returns>
    ///     The return value specifies the result of the message processing; it depends on the message sent.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int SendMessage(nint hWnd, WM wMsg, int wParam, int lParam);

    /// <summary>
    ///     Sends the specified message to a window or windows. The SendMessage function calls the window procedure for
    ///     the specified window and does not return until the window procedure has processed the message.
    ///     <para>
    ///         To send a message and return immediately, use the SendMessageCallback or SendNotifyMessage function. To
    ///         post a message to a thread's message queue and return immediately, use the PostMessage or
    ///         PostThreadMessage function.
    ///     </para>
    /// </summary>
    /// <param name="hWnd">
    ///     A handle to the window whose window procedure will receive the message. If this parameter is
    ///     HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, including
    ///     disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not
    ///     sent to child windows.
    ///     <para>
    ///         Message sending is subject to UIPI. The thread of a process can send messages only to message queues
    ///         of threads in processes of lesser or equal integrity level.
    ///     </para>
    /// </param>
    /// <param name="wMsg">The message to be sent.</param>
    /// <param name="wParam">Additional message-specific information.</param>
    /// <param name="lParam">lParam Additional message-specific information.</param>
    /// <returns>
    ///     The return value specifies the result of the message processing; it depends on the message sent.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern int SendMessage(nint hWnd, WM wMsg, int wParam, ref POINT lParam);

    /// <summary>
    ///     Sets the specified window's show state.
    /// </summary>
    /// <param name="hWnd">A handle to the window.</param>
    /// <param name="nCmdShow">
    ///     Controls how the window is to be shown. This parameter is ignored the first time an application calls
    ///     ShowWindow, if the program that launched the application provides a STARTUPINFO structure. Otherwise,
    ///     the first time ShowWindow is called, the value should be the value obtained by the WinMain function in
    ///     its nCmdShow parameter. In subsequent calls, this parameter can be one of the following values.
    /// </param>
    /// <returns>
    ///     If the window was previously visible, the return value is nonzero. If the window was previously hidden,
    ///     the return value is zero.
    /// </returns>
    [DllImport("user32.dll", SetLastError = true)]
    public static extern bool ShowWindow(IntPtr hWnd, SW nCmdShow);

    /// <summary>
    ///     Retrieves a handle to the window that contains the specified point.
    ///     <para>
    ///         The WindowFromPoint function does not retrieve a handle to a hidden or disabled window, even if the
    ///         point is within the window. An application should use the ChildWindowFromPoint function for a
    ///         nonrestrictive search.
    ///     </para>
    /// </summary>
    /// <param name="point">The point to be checked.</param>
    /// <returns>
    ///     The return value is a handle to the window that contains the point. If no window exists at the given
    ///     point, the return value is NULL. If the point is over a static text control, the return value is a
    ///     handle to the window under the static text control.
    /// </returns>
    [DllImport("user32.dll")]
    public static extern nint WindowFromPoint(POINT point);
}
