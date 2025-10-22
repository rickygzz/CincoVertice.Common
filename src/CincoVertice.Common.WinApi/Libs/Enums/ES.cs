namespace CincoVertice.Common.WinApi.Libs.Enums;

public enum ES : uint
{
    WM_USER = 0x400,

    EM_GETSCROLLPOS = WM_USER + 221,

    EM_SETSCROLLPOS = WM_USER + 222,

    EM_GETEVENTMASK = WM_USER + 59,

    EM_SETEVENTMASK = WM_USER + 69,
}
