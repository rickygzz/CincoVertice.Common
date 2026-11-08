using CincoVertice.Common.WinApi.Libs.Enums;
using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;

public static class Advapi32
{
    /// <summary>
    ///     The LogonUser function attempts to log a user on to the local computer.
    ///     The local computer is the computer from which LogonUser was called.
    ///     You cannot use LogonUser to log on to a remote computer.
    ///     You specify the user with a user name and domain and authenticate the user with a plaintext password. If
    ///     the function succeeds, you receive a handle to a token that represents the logged-on user. You can then
    ///     use this token handle to impersonate the specified user or, in most cases, to create a process that runs
    ///     in the context of the specified user.
    /// </summary>
    /// <param name="pszUserName">
    ///     This is the name of the user account to log on to. If you use the user principal name (UPN) format,
    ///     User@DNSDomainName, the lpszDomain parameter must be NULL.
    /// </param>
    /// <param name="pszDomain">
    ///     Specifies the name of the domain or server whose account database contains the lpszUsername account. If
    ///     this parameter is NULL, the user name must be specified in UPN format. If this parameter is ".", the
    ///     function validates the account by using only the local account database.
    /// </param>
    /// <param name="pszPassword">
    ///     Specifies the plaintext password for the user account specified by lpszUsername. When you have finished
    ///     using the password, clear the password from memory by calling the SecureZeroMemory function.
    /// </param>
    /// <param name="dwLogonType">The type of logon operation to perform.</param>
    /// <param name="dwLogonProvider">Specifies the logon provider.</param>
    /// <param name="phToken">
    ///     A pointer to a handle variable that receives a handle to a token that represents the specified user.
    /// </param>
    /// <returns>If the function succeeds, the function returns nonzero.</returns>
    [DllImport("advapi32.dll", SetLastError = true, BestFitMapping = false, ThrowOnUnmappableChar = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool LogonUser(
        [MarshalAs(UnmanagedType.LPStr)] string pszUserName,
        [MarshalAs(UnmanagedType.LPStr)] string pszDomain,
        [MarshalAs(UnmanagedType.LPStr)] string pszPassword,
        LogonType dwLogonType,
        LogonProvider dwLogonProvider,
        ref nint phToken);
}
