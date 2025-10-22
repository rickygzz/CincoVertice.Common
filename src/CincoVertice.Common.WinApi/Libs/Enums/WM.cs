namespace CincoVertice.Common.WinApi.Libs.Enums;

/// <summary>
///     Window Miessages.
/// </summary>
public enum WM : uint
{
    /// <summary>
    ///     Performs no operation. An application sends the WM_NULL message if it wants to post a message that the
    ///     recipient window will ignore. A window receives this message through its WindowProc function.
    /// </summary>
    WM_NULL = 0x0000,

    /// <summary>
    ///     Sent when an application requests that a window be created by calling the CreateWindowEx or CreateWindow
    ///     function. (The message is sent before the function returns.) The window procedure of the new window
    ///     receives this message after the window is created, but before the window becomes visible. A window
    ///     receives this message through its WindowProc function.
    /// </summary>
    WM_CREATE = 0x0001,

    /// <summary>
    ///     Sent when a window is being destroyed. It is sent to the window procedure of the window being destroyed
    ///     after the window is removed from the screen. This message is sent first to the window being destroyed
    ///     and then to the child windows (if any) as they are destroyed.During the processing of the message, it
    ///     can be assumed that all child windows still exist. A window receives this message through its WindowProc
    ///     function.
    /// </summary>
    WM_DESTROY = 0x0002,

    /// <summary>
    ///     Sent after a window has been moved. A window receives this message through its WindowProc function.
    /// </summary>
    WM_MOVE = 0x0003,

    /// <summary>
    ///     Sent to a window after its size has changed. A window receives this message through its WindowProc
    ///     function.
    /// </summary>
    WM_SIZE = 0x0005,

    /// <summary>
    ///     Sent to both the window being activated and the window being deactivated. If the windows use the same
    ///     input queue, the message is sent synchronously, first to the window procedure of the top-level window
    ///     being deactivated, then to the window procedure of the top-level window being activated. If the windows
    ///     use different input queues, the message is sent asynchronously, so the window is activated immediately.
    /// </summary>
    WM_ACTIVATE = 0x0006,

    /// <summary>Sent to a window after it has gained the keyboard focus.</summary>
    WM_SETFOCUS = 0x0007,

    /// <summary>Sent to a window immediately before it loses the keyboard focus.</summary>
    WM_KILLFOCUS = 0x0008,

    /// <summary>
    ///     Sent when an application changes the enabled state of a window. It is sent to the window whose enabled
    ///     state is changing. This message is sent before the EnableWindow function returns, but after the enabled
    ///     state (WS_DISABLED style bit) of the window has changed. A window receives this message through its
    ///     WindowProc function.
    /// </summary>
    WM_ENABLE = 0x000A,

    /// <summary>
    ///     An application sends the WM_SETREDRAW message to a window to allow changes in that window to be redrawn or
    ///     to prevent changes in that window from being redrawn.
    ///     <para>To send this message, call the SendMessage function with the following parameters.</para>
    /// </summary>
    WM_SETREDRAW = 11,

    /// <summary>Sets the text of a window.</summary>
    WM_SETTEXT = 0x000C,

    /// <summary>Copies the text that corresponds to a window into a buffer provided by the caller.</summary>
    WM_GETTEXT = 0x000D,

    /// <summary>Determines the length, in characters, of the text associated with a window.</summary>
    WM_GETTEXTLENGTH = 0x000E,

    /// <summary>
    ///     The WM_PAINT message is sent when the system or another application makes a request to paint a portion of an
    ///     application's window. The message is sent when the UpdateWindow or RedrawWindow function is called, or by
    ///     the DispatchMessage function when the application obtains a WM_PAINT message by using the GetMessage or
    ///     PeekMessage function.
    ///     <para>A window receives this message through its WindowProc function.</para>
    /// </summary>
    WM_PAINT = 15,

    /// <summary>
    ///     Sent as a signal that a window or an application should terminate. A window receives this message
    ///     through its WindowProc function.
    /// </summary>
    WM_CLOSE = 0x0010,

    /// <summary>
    ///     The WM_QUERYENDSESSION message is sent when the user chooses to end the session or when an application calls
    ///     one of the system shutdown functions. If any application returns zero, the session is not ended. The system
    ///     stops sending WM_QUERYENDSESSION messages as soon as one application returns zero.
    ///     <para>
    ///         After processing this message, the system sends the WM_ENDSESSION message with the wParam parameter set
    ///         to the results of the WM_QUERYENDSESSION message.
    ///     </para>
    ///     <para>A window receives this message through its WindowProc function.</para>
    /// </summary>
    WM_QUERYENDSESSION = 17,

    /// <summary>
    ///     Indicates a request to terminate an application, and is generated when the application calls the
    ///     PostQuitMessage function. This message causes the GetMessage function to return zero.
    /// </summary>
    WM_QUIT = 0x0012,

    /// <summary>
    ///     Sent to an icon when the user requests that the window be restored to its previous size and position.
    ///     A window receives this message through its WindowProc function
    /// </summary>
    WM_QUERYOPEN = 0x0013,

    /// <summary>
    ///     Sent when the window background must be erased (for example, when a window is resized). The message is
    ///     sent to prepare an invalidated portion of a window for painting.
    /// </summary>
    WM_ERASEBKGND = 0x0014,

    /// <summary>
    ///     The WM_SYSCOLORCHANGE message is sent to all top-level windows when a change is made to a system color
    ///     setting.
    ///     <para>A window receives this message through its WindowProc function.</para>
    /// </summary>
    WM_SYSCOLORCHANGE = 21,

    /// <summary>
    ///     The WM_ENDSESSION message is sent to an application after the system processes the results of the
    ///     WM_QUERYENDSESSION message. The WM_ENDSESSION message informs the application whether the session is ending.
    ///     <para>A window receives this message through its WindowProc function.</para>
    /// </summary>
    WM_ENDSESSION = 22,

    /// <summary>
    ///     Sent to a window when the window is about to be hidden or shown. A window receives this message
    ///     through its WindowProc function.
    /// </summary>
    WM_SHOWWINDOW = 0x0018,

    WM_CTLCOLOR = 25,

    WM_WININICHANGE = 26,

    WM_DEVMODECHANGE = 27,

    /// <summary>
    ///     Sent when a window belonging to a different application than the active window is about to be activated.
    ///     The message is sent to the application whose window is being activated and to the application whose
    ///     window is being deactivated. A window receives this message through its WindowProc function.
    /// </summary>
    WM_ACTIVATEAPP = 0x001C,

    WM_FONTCHANGE = 29,

    WM_TIMECHANGE = 30,

    /// <summary>
    ///     Sent to cancel certain modes, such as mouse capture. For example, the system sends this message to the
    ///     active window when a dialog box or message box is displayed. Certain functions also send this message
    ///     explicitly to the specified window regardless of whether it is the active window. For example, the
    ///     EnableWindow function sends this message when disabling the specified window. A window receives this
    ///     message through its WindowProc function.
    /// </summary>
    WM_CANCELMODE = 0x001F,

    WM_SETCURSOR = 32,

    WM_MOUSEACTIVATE = 33,

    /// <summary>
    ///     Sent to a child window when the user clicks the window's title bar or when the window is activated,
    ///     moved, or sized. A window receives this message through its WindowProc function.
    /// </summary>
    WM_CHILDACTIVATE = 0x0022,

    WM_QUEUESYNC = 35,

    /// <summary>
    ///     Sent to a window when the size or position of the window is about to change. An application can use this
    ///     message to override the window's default maximized size and position, or its default minimum or maximum
    ///     tracking size. A window receives this message through its WindowProc function.
    /// </summary>
    WM_GETMINMAXINFO = 0x0024,

    WM_PAINTICON = 38,

    WM_ICONERASEBKGND = 39,

    WM_NEXTDLGCTL = 40,

    WM_SPOOLERSTATUS = 42,

    WM_DRAWITEM = 43,

    WM_MEASUREITEM = 44,

    WM_DELETEITEM = 45,

    WM_VKEYTOITEM = 46,

    WM_CHARTOITEM = 47,

    /// <summary>Sets the font that a control is to use when drawing text.</summary>
    WM_SETFONT = 0x0030,

    /// <summary>Retrieves the font with which the control is currently drawing its text.</summary>
    WM_GETFONT = 0x0031,

    WM_SETHOTKEY = 50,

    WM_GETHOTKEY = 51,

    /// <summary>
    ///     Sent to a minimized (iconic) window. The window is about to be dragged by the user but does not have an
    ///     icon defined for its class. An application can return a handle to an icon or cursor. The system displays
    ///     this cursor or icon while the user drags the icon. A window receives this message through its WindowProc
    ///     function.
    /// </summary>
    WM_QUERYDRAGICON = 0x0037,

    WM_COMPAREITEM = 57,

    WM_GETOBJECT = 61,

    WM_COMPACTING = 65,

    WM_COMMNOTIFY = 68,

    /// <summary>
    ///     Sent to a window whose size, position, or place in the Z order is about to change as a result of a call
    ///     to the SetWindowPos function or another window-management function. A window receives this message
    ///     through its WindowProc function.
    /// </summary>
    WM_WINDOWPOSCHANGING = 0x0046,

    /// <summary>
    ///     Sent to a window whose size, position, or place in the Z order has changed as a result of a call to the
    ///     SetWindowPos function or another window-management function. A window receives this message through its
    ///     WindowProc function.
    /// </summary>
    WM_WINDOWPOSCHANGED = 0x0047,

    WM_POWER = 72,

    WM_COPYGLOBALDATA = 73,

    WM_COPYDATA = 74,

    WM_CANCELJOURNAL = 75,

    WM_NOTIFY = 78,

    /// <summary>
    ///     Posted to the window with the focus when the user chooses a new input language, either with the hotkey
    ///     (specified in the Keyboard control panel application) or from the indicator on the system taskbar. An
    ///     application can accept the change by passing the message to the DefWindowProc function or reject the
    ///     change (and prevent it from taking place) by returning immediately. A window receives this message
    ///     through its WindowProc function.
    /// </summary>
    WM_INPUTLANGCHANGEREQUEST = 0x0050,

    /// <summary>
    ///     Sent to the topmost affected window after an application's input language has been changed. You should
    ///     make any application-specific settings and pass the message to the DefWindowProc function, which passes
    ///     the message to all first-level child windows. These child windows can pass the message to DefWindowProc
    ///     to have it pass the message to their child windows, and so on. A window receives this message through
    ///     its WindowProc function.
    /// </summary>
    WM_INPUTLANGCHANGE = 0x0051,

    WM_TCARD = 82,

    WM_HELP = 83,

    /// <summary>
    ///     Sent to all windows after the user has logged on or off. When the user logs on or off, the system
    ///     updates the user-specific settings. The system sends this message immediately after updating the
    ///     settings. A window receives this message through its WindowProc function.
    /// </summary>
    WM_USERCHANGED = 0x0054,

    WM_NOTIFYFORMAT = 85,

    WM_CONTEXTMENU = 123,

    WM_STYLECHANGING = 0x007C,

    /// <summary>
    ///     Sent to a window after the SetWindowLong function has changed one or more of the window's styles. A
    ///     window receives this message through its WindowProc function.
    /// </summary>
    WM_STYLECHANGED = 0x007D,

    WM_DISPLAYCHANGE = 126,

    /// <summary>
    ///     Sent to a window to retrieve a handle to the large or small icon associated with a window. The system
    ///     displays the large icon in the ALT+TAB dialog, and the small icon in the window caption. A window
    ///     receives this message through its WindowProc function.
    /// </summary>
    WM_GETICON = 0x007F,

    /// <summary>
    ///     Associates a new large or small icon with a window. The system displays the large icon in the ALT+TAB
    ///     dialog box, and the small icon in the window caption.
    /// </summary>
    WM_SETICON = 0x0080,

    /// <summary>
    ///     Sent prior to the WM_CREATE message when a window is first created. A window receives this message
    ///     through its WindowProc function.
    /// </summary>
    WM_NCCREATE = 0x0081,

    /// <summary>
    ///     Notifies a window that its nonclient area is being destroyed. The DestroyWindow function sends the
    ///     WM_NCDESTROY message to the window following the WM_DESTROY message. WM_DESTROY is used to free the
    ///     allocated memory object associated with the window. The WM_NCDESTROY message is sent after the child
    ///     windows have been destroyed. In contrast, WM_DESTROY is sent before the child windows are destroyed. A
    ///     window receives this message through its WindowProc function.
    /// </summary>
    WM_NCDESTROY = 0x0082,

    /// <summary>
    ///     Sent when the size and position of a window's client area must be calculated. By processing this
    ///     message, an application can control the content of the window's client area when the size or position of
    ///     the window changes. A window receives this message through its WindowProc function.
    /// </summary>
    WM_NCCALCSIZE = 0x0083,

    WM_NCHITTEST = 132,

    WM_NCPAINT = 133,

    /// <summary>
    ///     Sent to a window when its nonclient area needs to be changed to indicate an active or inactive state. A
    ///     window receives this message through its WindowProc function.
    /// </summary>
    WM_NCACTIVATE = 0x0086,

    WM_GETDLGCODE = 135,

    WM_SYNCPAINT = 136,

    WM_NCMOUSEMOVE = 160,

    WM_NCLBUTTONDOWN = 161,

    WM_NCLBUTTONUP = 162,

    WM_NCLBUTTONDBLCLK = 163,

    WM_NCRBUTTONDOWN = 164,

    WM_NCRBUTTONUP = 165,

    WM_NCRBUTTONDBLCLK = 166,

    WM_NCMBUTTONDOWN = 167,

    WM_NCMBUTTONUP = 168,

    WM_NCMBUTTONDBLCLK = 169,

    WM_NCXBUTTONDOWN = 171,

    WM_NCXBUTTONUP = 172,

    WM_NCXBUTTONDBLCLK = 173,

    WM_INPUT = 255,

    WM_KEYDOWN = 256,

    WM_KEYUP = 257,

    WM_CHAR = 258,

    WM_DEADCHAR = 259,

    WM_SYSKEYDOWN = 260,

    WM_SYSKEYUP = 261,

    WM_SYSCHAR = 262,

    WM_SYSDEADCHAR = 263,

    WM_KEYLAST = 264,

    WM_UNICHAR = 265,

    WM_WNT_CONVERTREQUESTEX = 265,

    WM_CONVERTREQUEST = 266,

    WM_CONVERTRESULT = 267,

    WM_INTERIM = 268,

    WM_IME_STARTCOMPOSITION = 269,

    WM_IME_ENDCOMPOSITION = 270,

    WM_IME_COMPOSITION = 271,

    WM_INITDIALOG = 272,

    WM_COMMAND = 273,

    WM_SYSCOMMAND = 274,

    WM_TIMER = 275,

    WM_HSCROLL = 276,

    WM_VSCROLL = 277,

    WM_INITMENU = 278,

    WM_INITMENUPOPUP = 279,

    WM_SYSTIMER = 280,

    WM_MENUSELECT = 287,

    WM_MENUCHAR = 288,

    WM_ENTERIDLE = 289,

    WM_MENURBUTTONUP = 290,

    WM_MENUDRAG = 291,

    WM_MENUGETOBJECT = 292,

    WM_UNINITMENUPOPUP = 293,

    WM_MENUCOMMAND = 294,

    WM_CHANGEUISTATE = 295,

    WM_UPDATEUISTATE = 296,

    WM_QUERYUISTATE = 297,

    WM_CTLCOLORMSGBOX = 306,

    WM_CTLCOLOREDIT = 307,

    WM_CTLCOLORLISTBOX = 308,

    WM_CTLCOLORBTN = 309,

    WM_CTLCOLORDLG = 310,

    WM_CTLCOLORSCROLLBAR = 311,

    WM_CTLCOLORSTATIC = 312,

    WM_MOUSEFIRST = 512,

    WM_MOUSEMOVE = 512,

    WM_LBUTTONDOWN = 513,

    WM_LBUTTONUP = 514,

    WM_LBUTTONDBLCLK = 515,

    WM_RBUTTONDOWN = 516,

    WM_RBUTTONUP = 517,

    WM_RBUTTONDBLCLK = 518,

    WM_MBUTTONDOWN = 519,

    WM_MBUTTONUP = 520,

    WM_MBUTTONDBLCLK = 521,

    WM_MOUSELAST = 521,

    WM_MOUSEWHEEL = 522,

    WM_XBUTTONDOWN = 523,

    WM_XBUTTONUP = 524,

    WM_XBUTTONDBLCLK = 525,

    WM_PARENTNOTIFY = 528,

    WM_ENTERMENULOOP = 529,

    WM_EXITMENULOOP = 530,

    WM_NEXTMENU = 531,

    /// <summary>
    ///     Sent to a window that the user is resizing. By processing this message, an application can monitor the
    ///     size and position of the drag rectangle and, if needed, change its size or position. A window receives
    ///     this message through its WindowProc function.
    /// </summary>
    WM_SIZING = 0x0214,

    WM_CAPTURECHANGED = 533,

    /// <summary>
    ///     Sent to a window that the user is moving. By processing this message, an application can monitor the
    ///     position of the drag rectangle and, if needed, change its position. A window receives this message
    ///     through its WindowProc function.
    /// </summary>
    WM_MOVING = 0x0216,

    WM_POWERBROADCAST = 536,

    WM_DEVICECHANGE = 537,

    WM_MDICREATE = 544,

    WM_MDIDESTROY = 545,

    WM_MDIACTIVATE = 546,

    WM_MDIRESTORE = 547,

    WM_MDINEXT = 548,

    WM_MDIMAXIMIZE = 549,

    WM_MDITILE = 550,

    WM_MDICASCADE = 551,

    WM_MDIICONARRANGE = 552,

    WM_MDIGETACTIVE = 553,

    WM_MDISETMENU = 560,

    /// <summary>
    ///     Sent one time to a window after it enters the moving or sizing modal loop. The window enters the moving
    ///     or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window
    ///     passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message
    ///     specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns. The system
    ///     sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows is enabled. A
    ///     window receives this message through its WindowProc function.
    /// </summary>
    WM_ENTERSIZEMOVE = 0x0231,

    /// <summary>
    ///     Sent one time to a window, after it has exited the moving or sizing modal loop. The window enters the
    ///     moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the
    ///     window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the
    ///     message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns.
    ///     A window receives this message through its WindowProc function.
    /// </summary>
    WM_EXITSIZEMOVE = 0x0232,

    WM_DROPFILES = 563,

    WM_MDIREFRESHMENU = 564,

    WM_IME_REPORT = 640,

    WM_IME_SETCONTEXT = 641,

    WM_IME_NOTIFY = 642,

    WM_IME_CONTROL = 643,

    WM_IME_COMPOSITIONFULL = 644,

    WM_IME_SELECT = 645,

    WM_IME_CHAR = 646,

    WM_IME_REQUEST = 648,

    WM_IMEKEYDOWN = 656,

    WM_IME_KEYDOWN = 656,

    WM_IMEKEYUP = 657,

    WM_IME_KEYUP = 657,

    WM_NCMOUSEHOVER = 672,

    WM_MOUSEHOVER = 673,

    WM_NCMOUSELEAVE = 674,

    WM_MOUSELEAVE = 675,

    WM_CUT = 768,

    WM_COPY = 769,

    WM_PASTE = 770,

    WM_CLEAR = 771,

    WM_UNDO = 772,

    WM_RENDERFORMAT = 773,

    WM_RENDERALLFORMATS = 774,

    WM_DESTROYCLIPBOARD = 775,

    WM_DRAWCLIPBOARD = 776,

    WM_PAINTCLIPBOARD = 777,

    WM_VSCROLLCLIPBOARD = 778,

    WM_SIZECLIPBOARD = 779,

    WM_ASKCBFORMATNAME = 780,

    WM_CHANGECBCHAIN = 781,

    WM_HSCROLLCLIPBOARD = 782,

    WM_QUERYNEWPALETTE = 783,

    WM_PALETTEISCHANGING = 784,

    WM_PALETTECHANGED = 785,

    WM_HOTKEY = 786,

    WM_PRINT = 791,

    WM_PRINTCLIENT = 792,

    WM_APPCOMMAND = 793,

    /// <summary>
    ///     Broadcast to every window following a theme change event. Examples of theme change events are the
    ///     activation of a theme, the deactivation of a theme, or a transition from one theme to another.
    /// </summary>
    WM_THEMECHANGED = 0x031A,

    WM_HANDHELDFIRST = 856,

    WM_HANDHELDLAST = 863,

    WM_AFXFIRST = 864,

    WM_AFXLAST = 895,

    WM_PENWINFIRST = 896,

    WM_RCRESULT = 897,

    WM_HOOKRCRESULT = 898,

    WM_GLOBALRCCHANGE = 899,

    WM_PENMISCINFO = 899,

    WM_SKB = 900,

    WM_HEDITCTL = 901,

    WM_PENCTL = 901,

    WM_PENMISC = 902,

    WM_CTLINIT = 903,

    WM_PENEVENT = 904,

    WM_PENWINLAST = 911,

    WM_USER = 0x400,
}
