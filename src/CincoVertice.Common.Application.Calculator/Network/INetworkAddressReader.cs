namespace CincoVertice.Common.Application.Calculator.Network;

/// <summary>
///     Reads the computer's network addresses, for the calculator's ipconfig().
/// </summary>
public interface INetworkAddressReader
{
    /// <returns>The addresses of the adapters that are connected; none when there's no network.</returns>
    IReadOnlyList<NetworkAddressModel> Read();
}
