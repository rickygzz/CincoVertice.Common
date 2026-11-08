using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs.Structs;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
public struct WNDCLASSEX
{
    [MarshalAs(UnmanagedType.U4)]
    public uint cbSize;
    [MarshalAs(UnmanagedType.U4)]
    public uint style;
    public nint lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public nint hInstance;
    public nint hIcon;
    public nint hCursor;
    public nint hbrBackground;
    [MarshalAs(UnmanagedType.LPStr)]
    public string lpszMenuName;
    [MarshalAs(UnmanagedType.LPStr)]
    public string lpszClassName;
    public nint hIconSm;
}
