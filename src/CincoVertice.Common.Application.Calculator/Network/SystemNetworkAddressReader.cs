using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace CincoVertice.Common.Application.Calculator.Network;

/// <summary>
///     The addresses the system's network adapters have now: IPv4 ones, or else IPv6 ones that aren't only for the
///     adapter's own link. Adapters that are off, and the loopback, are left out.
/// </summary>
public sealed class SystemNetworkAddressReader : INetworkAddressReader
{
    public IReadOnlyList<NetworkAddressModel> Read()
    {
        List<NetworkAddressModel> addresses = [];

        try
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (adapter.OperationalStatus != OperationalStatus.Up
                    || adapter.NetworkInterfaceType is NetworkInterfaceType.Loopback or NetworkInterfaceType.Tunnel)
                {
                    continue;
                }

                IPInterfaceProperties properties = adapter.GetIPProperties();
                string? gateway = properties.GatewayAddresses
                    .Select(gatewayAddress => gatewayAddress.Address)
                    .FirstOrDefault(address => address.AddressFamily == AddressFamily.InterNetwork
                        && !address.Equals(IPAddress.Any))?
                    .ToString();
                List<IPAddress> own = [.. properties.UnicastAddresses.Select(unicast => unicast.Address)];
                List<IPAddress> shown = [.. own.Where(address => address.AddressFamily == AddressFamily.InterNetwork)];

                if (shown.Count == 0)
                {
                    shown = [.. own.Where(address => address.AddressFamily == AddressFamily.InterNetworkV6
                        && !address.IsIPv6LinkLocal)];
                }

                addresses.AddRange(shown.Select(address =>
                    new NetworkAddressModel(adapter.Name, address.ToString(), gateway)));
            }
        }
        catch (NetworkInformationException)
        {
            // The system couldn't say: as with no network
        }

        return addresses;
    }
}
