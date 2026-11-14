namespace CincoVertice.Common.Application.Calculator.Network;

/// <summary>
///     What ipconfig() shows: each address with its gateway and adapter, 192.168.1.23, gateway 192.168.1.1 (Wi-Fi).
/// </summary>
public static class NetworkConfigText
{
    public const string NoNetwork = "No network connection";

    /// <summary>
    ///     Only the addresses with a gateway, which reach beyond the computer, when there are any: the others are
    ///     usually virtual adapters, such as WSL's, which would hide the one wanted. Several are separated by "; ".
    /// </summary>
    public static string Of(IReadOnlyList<NetworkAddressModel> addresses)
    {
        List<NetworkAddressModel> withGateway = [.. addresses.Where(address => address.Gateway is not null)];
        IReadOnlyList<NetworkAddressModel> shown = withGateway.Count > 0 ? withGateway : addresses;

        if (shown.Count == 0)
        {
            return NoNetwork;
        }

        return string.Join("; ", shown.Select(address => address.Gateway is null
            ? $"{address.Address} ({address.Adapter})"
            : $"{address.Address}, gateway {address.Gateway} ({address.Adapter})"));
    }
}
