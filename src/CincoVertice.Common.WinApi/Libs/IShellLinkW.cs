using CincoVertice.Common.WinApi.Libs.Enums;
using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;

/// <summary>
///     IShellLinkW. Interface to create shortcuts.
/// </summary>
public class IShellLinkW
{
    [ComImport]
    [Guid("0000010c-0000-0000-c000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersist
    {
        [PreserveSig]
        void GetClassID(out Guid pClassID);
    }

    [ComImport]
    [Guid("0000010b-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPersistFile : IPersist
    {
        new void GetClassID(out Guid pClassID);

        [PreserveSig]
        int IsDirty();

        [PreserveSig]
        void Load(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            uint dwMode);

        [PreserveSig]
        void Save(
            [MarshalAs(UnmanagedType.LPWStr)] string pszFileName,
            [MarshalAs(UnmanagedType.Bool)] bool fRemember);

        [PreserveSig]
        void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);

        [PreserveSig]
        void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] string ppszFileName);
    }

    /// <summary>
    ///     Exposes methods that create, modify, and resolve Shell links.
    /// </summary>
    [ComImport]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IShellLink
    {
        /// <summary>
        /// Gets the path and file name of the target of a Shell link object.
        /// </summary>
        /// <param name="pszFile">
        ///     The address of a buffer that receives the path and file name of the target of the Shell link object.
        /// </param>
        /// <param name="cch">
        ///     The size, in characters, of the buffer pointed to by the pszFile parameter, including the
        ///     terminating null character. The maximum path size that can be returned is MAX_PATH. This parameter
        ///     is commonly set by calling ARRAYSIZE(pszFile). The ARRAYSIZE macro is defined in Winnt.h.
        /// </param>
        /// <param name="pfd">
        ///     A pointer to a WIN32_FIND_DATA structure that receives information about the target of the Shell
        ///     link object. If this parameter is NULL, then no additional information is returned.
        /// </param>
        /// <param name="fFlags">
        ///     Flags that specify the type of path information to retrieve. This parameter can be a combination of
        ///     the following values.
        /// </param>
        [PreserveSig]
        void GetPath(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszFile,
            int cch,
            ref WIN32_FIND_DATA pfd,
            uint fFlags);

        /// <summary>
        /// Gets the list of item identifiers for the target of a Shell link object.
        /// </summary>
        /// <param name="ppidl">When this method returns, contains the address of a PIDL.</param>
        [PreserveSig]
        void GetIDList(out nint ppidl);

        /// <summary>
        /// Sets the pointer to an item identifier list (PIDL) for a Shell link object.
        /// </summary>
        /// <param name="ppidl">The object's fully qualified PIDL.</param>
        [PreserveSig]
        void SetIDList(nint ppidl);

        /// <summary>
        /// Gets the description string for a Shell link object.
        /// </summary>
        /// <param name="pszName">A pointer to the buffer that receives the description string.</param>
        /// <param name="cch">
        ///     The maximum number of characters to copy to the buffer pointed to by the pszName parameter.
        /// </param>
        [PreserveSig]
        void GetDescription(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszName,
            int cch);

        /// <summary>
        /// Sets the description for a Shell link object. The description can be any application-defined string.
        /// </summary>
        /// <param name="pszName">A pointer to a buffer containing the new description string.</param>
        [PreserveSig]
        void SetDescription(
            [MarshalAs(UnmanagedType.LPWStr)] string pszName);

        /// <summary>
        /// Gets the name of the working directory for a Shell link object.
        /// </summary>
        /// <param name="pszDir">The address of a buffer that receives the name of the working directory.</param>
        /// <param name="cch">
        ///     The maximum number of characters to copy to the buffer pointed to by the pszDir parameter. The name
        ///     of the working directory is truncated if it is longer than the maximum specified by this parameter.
        /// </param>
        [PreserveSig]
        void GetWorkingDirectory(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszDir,
            int cch);

        /// <summary>
        /// Sets the name of the working directory for a Shell link object.
        /// </summary>
        /// <param name="pszDir">
        ///     The address of a buffer that contains the name of the new working directory.
        /// </param>
        [PreserveSig]
        void SetWorkingDirectory(
            [MarshalAs(UnmanagedType.LPWStr)] string pszDir);

        /// <summary>
        /// Gets the command-line arguments associated with a Shell link object.
        /// </summary>
        /// <param name="pszArgs">
        ///     A pointer to the buffer that, when this method returns successfully, receives the command-line
        ///     arguments.
        /// </param>
        /// <param name="cch">
        ///     The maximum number of characters that can be copied to the buffer supplied by the pszArgs parameter.
        ///     In the case of a Unicode string, there is no limitation on maximum string length. In the case of an
        ///     ANSI string, the maximum length of the returned string varies depending on the version of
        ///     Windows—MAX_PATH prior to Windows 2000 and INFOTIPSIZE (defined in Commctrl.h) in Windows 2000 and
        ///     later.
        /// </param>
        [PreserveSig]
        void GetArguments(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszArgs,
            int cch);

        /// <summary>
        /// Sets the command-line arguments for a Shell link object.
        /// </summary>
        /// <param name="pszArgs">
        ///     A pointer to a buffer that contains the new command-line arguments. In the case of a Unicode string,
        ///     there is no limitation on maximum string length. In the case of an ANSI string, the maximum length
        ///     of the returned string varies depending on the version of Windows—MAX_PATH prior to Windows 2000 and
        ///     INFOTIPSIZE (defined in Commctrl.h) in Windows 2000 and later.
        /// </param>
        [PreserveSig]
        void SetArguments(
            [MarshalAs(UnmanagedType.LPWStr)] string pszArgs);

        /// <summary>
        /// Gets the keyboard shortcut (hot key) for a Shell link object.
        /// </summary>
        /// <param name="pwHotkey">
        ///     The address of the keyboard shortcut. The virtual key code is in the low-order byte, and the
        ///     modifier flags are in the high-order byte. The modifier flags can be a combination of the following
        ///     values.
        /// </param>
        [PreserveSig]
        void GetHotkey(out ushort pwHotkey);

        /// <summary>
        /// Sets a keyboard shortcut (hot key) for a Shell link object.
        /// </summary>
        /// <param name="wHotkey">
        ///     The new keyboard shortcut. The virtual key code is in the low-order byte, and the modifier flags are
        ///     in the high-order byte. The modifier flags can be a combination of the values specified in the
        ///     description of the IShellLink::GetHotkey method.
        /// </param>
        [PreserveSig]
        void SetHotkey(ushort wHotkey);

        /// <summary>
        /// Gets the show command for a Shell link object.
        /// </summary>
        /// <param name="piShowCmd">A pointer to the command. The following commands are supported.</param>
        [PreserveSig]
        void GetShowCmd(out ShowCmd piShowCmd);

        /// <summary>
        ///     Sets the show command for a Shell link object. The show command sets the initial show state of the
        ///     window.
        /// </summary>
        /// <param name="iShowCmd">Command. SetShowCmd accepts one of the following ShowWindow commands.</param>
        [PreserveSig]
        void SetShowCmd(ShowCmd iShowCmd);

        /// <summary>
        /// Gets the location (path and index) of the icon for a Shell link object.
        /// </summary>
        /// <param name="pszIconPath">
        ///     The address of a buffer that receives the path of the file containing the icon.
        /// </param>
        /// <param name="cch">
        ///     The maximum number of characters to copy to the buffer pointed to by the pszIconPath parameter.
        /// </param>
        /// <param name="piIcon">The address of a value that receives the index of the icon.</param>
        [PreserveSig]
        void GetIconLocation(
            [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 1)] out string pszIconPath,
            int cch,
            out int piIcon);

        /// <summary>
        /// Sets the location (path and index) of the icon for a Shell link object.
        /// </summary>
        /// <param name="pszIconPath">
        ///     The address of a buffer to contain the path of the file containing the icon.
        /// </param>
        /// <param name="iIcon">The index of the icon.</param>
        [PreserveSig]
        void SetIconLocation(
            [MarshalAs(UnmanagedType.LPWStr)] string pszIconPath,
            int iIcon);

        /// <summary>
        /// Sets the relative path to the Shell link object.
        /// </summary>
        /// <param name="pszPathRel">
        ///     The address of a buffer that contains the fully-qualified path of the shortcut file, relative to
        ///     which the shortcut resolution should be performed. It should be a file name, not a folder name.
        /// </param>
        /// <param name="dwReserved">Reserved. Set this parameter to zero.</param>
        [PreserveSig]
        void SetRelativePath(
            [MarshalAs(UnmanagedType.LPWStr)] string pszPathRel,
            uint dwReserved);

        /// <summary>
        /// Attempts to find the target of a Shell link, even if it has been moved or renamed.
        /// </summary>
        /// <param name="hwnd">
        ///     A handle to the window that the Shell will use as the parent for a dialog box. The Shell displays
        ///     the dialog box if it needs to prompt the user for more information while resolving a Shell link.
        /// </param>
        /// <param name="fFlags">Action flags. This parameter can be a combination of the following values.</param>
        [PreserveSig]
        void Resolve(nint hwnd, uint fFlags);

        /// <summary>
        /// Sets the path and file name for the target of a Shell link object.
        /// </summary>
        /// <param name="pszFile">The address of a buffer that contains the new path.</param>
        [PreserveSig]
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [Guid("00021401-0000-0000-C000-000000000046")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComImport()]
    public class CShellLink
    {
    }

    /// <summary>
    /// Create Shortcut.
    /// </summary>
    /// <param name="shortcutPath">The shortcut path. File extension must = .lnk .</param>
    /// <param name="targetPath">The target path.</param>
    /// <param name="description">Shortcut key description.</param>
    /// <param name="iconLocation">Icon location. Ignored if null or empty.</param>
    /// <returns>true if successful, otherwise false.</returns>
    public static bool CreateShortcut(
        string shortcutPath,
        string targetPath,
        string description,
        string iconLocation = null!)
    {
        return CreateShortcut(shortcutPath, targetPath, string.Empty, string.Empty, description, iconLocation);
    }

    /// <summary>
    /// Create Shortcut.
    /// </summary>
    /// <param name="shortcutPath">The shortcut path. File extension must = .lnk .</param>
    /// <param name="targetPath">Sets the path and file name for the target of a Shell link object.</param>
    /// <param name="arguments">Sets the command-line arguments for a Shell link object.</param>
    /// <param name="workingDirectory">Sets the name of the working directory for a Shell link object.</param>
    /// <param name="description">Shortcut key description.</param>
    /// <param name="iconLocation">Icon location. Ignored if null or empty.</param>
    /// <param name="showcmd">
    ///     Sets the show command for a Shell link object. The show command sets the initial show state of the
    ///     window.
    /// </param>
    /// <returns>true if successful, otherwise false.</returns>
    public static bool CreateShortcut(
        string shortcutPath,
        string targetPath,
        string arguments,
        string workingDirectory,
        string description,
        string iconLocation = null!,
        ShowCmd showcmd = ShowCmd.SW_SHOWNORMAL)
    {
        try
        {
            CShellLink cShellLink = new CShellLink();
            IShellLink iShellLink = (IShellLink)cShellLink;
            iShellLink.SetDescription(description);
            iShellLink.SetShowCmd(showcmd);
            iShellLink.SetPath(targetPath);

            if (string.IsNullOrEmpty(arguments) == false)
            {
                iShellLink.SetArguments(arguments);
            }

            if (string.IsNullOrEmpty(workingDirectory) == false)
            {
                iShellLink.SetWorkingDirectory(workingDirectory);
            }
            else
            {
                iShellLink.SetWorkingDirectory(Path.GetDirectoryName(targetPath) ?? string.Empty);
            }

            if (string.IsNullOrEmpty(iconLocation) == false)
            {
                iShellLink.SetIconLocation(iconLocation, 0);
            }

            IPersistFile iPersistFile = (IPersistFile)iShellLink;
            iPersistFile.Save(shortcutPath, false);

            // Release COM objects
            Marshal.ReleaseComObject(iPersistFile);
            Marshal.ReleaseComObject(iShellLink);
            Marshal.ReleaseComObject(cShellLink);

            return true;
        }
        catch // (System.Exception ex)
        {
            return false;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FILETIME
    {
        uint dwLowDateTime;
        uint dwHighDateTime;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct WIN32_FIND_DATA
    {
        public const int MAX_PATH = 260;

        uint dwFileAttributes;
        FILETIME ftCreationTime;
        FILETIME ftLastAccessTime;
        FILETIME ftLastWriteTime;
        uint nFileSizeHight;
        uint nFileSizeLow;
        uint dwOID;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MAX_PATH)]
        string cFileName;
    }
}
