namespace CincoVertice.Common.Application.Calculator.Network;

/// <summary>
///     An address this computer has on a network.
/// </summary>
/// <param name="Adapter">The network adapter's name, as Windows shows it: Wi-Fi, Ethernet.</param>
/// <param name="Address">The address: 192.168.1.23.</param>
/// <param name="Gateway">Where its traffic leaves the network: 192.168.1.1; null for none.</param>
public sealed record NetworkAddressModel(string Adapter, string Address, string? Gateway);
