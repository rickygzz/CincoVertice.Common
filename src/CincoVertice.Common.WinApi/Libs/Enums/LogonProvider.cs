namespace CincoVertice.Common.WinApi.Libs.Enums;

/// <summary>
///     Specifies the logon provider.
/// </summary>
public enum LogonProvider : uint
{
    /// <summary>
    ///     Use the standard logon provider for the system. The default security provider is negotiate, unless you pass
    ///     NULL for the domain name and the user name is not in UPN format. In this case, the default provider is NTLM.
    /// </summary>
    LOGON32_PROVIDER_DEFAULT = 0,
}
