using CincoVertice.Common.Application.Calculator.Models;
using CincoVertice.Common.Application.Calculator.Network;
using Xunit;

namespace CincoVertice.Common.Application.Calculator.Tests;

public class IpConfigTests
{
    private static readonly NetworkAddressModel WiFi = new("Wi-Fi", "192.168.1.23", "192.168.1.1");

    private static ExpressionCalculator CalculatorWith(params NetworkAddressModel[] addresses) =>
        new(TimeProvider.System, new FakeNetwork(addresses));

    [Fact]
    public void IpConfig_IsTheAddressAndGateway_OfTheAdapter()
    {
        // Arrange
        ExpressionCalculator calculator = CalculatorWith(WiFi);

        // Act
        CalculationResult result = calculator.Calculate("IPCONFIG()");

        // Assert: any letter case, and a text that can be kept
        Assert.Equal("192.168.1.23, gateway 192.168.1.1 (Wi-Fi)", result.Text);
        Assert.Equal(CalculatorValue.FromText(result.Text), result.Value);
    }

    [Fact]
    public void IpConfig_ShowsOnlyTheAdaptersWithAGateway_WhenThereAreAny()
    {
        // Arrange: WSL's virtual adapter has none
        ExpressionCalculator calculator = CalculatorWith(
            new NetworkAddressModel("vEthernet (WSL)", "172.20.0.1", null),
            new NetworkAddressModel("Ethernet", "10.0.0.5", "10.0.0.1"),
            WiFi);

        // Act / Assert
        Assert.Equal(
            "10.0.0.5, gateway 10.0.0.1 (Ethernet); 192.168.1.23, gateway 192.168.1.1 (Wi-Fi)",
            calculator.Calculate("ipconfig()").Text);
    }

    [Theory]
    [InlineData(false, "No network connection")]
    [InlineData(true, "169.254.10.2 (Ethernet)")]
    public void IpConfig_WithoutAGateway_ShowsWhatThereIs(bool hasAddress, string expected)
    {
        // Arrange
        ExpressionCalculator calculator = hasAddress
            ? CalculatorWith(new NetworkAddressModel("Ethernet", "169.254.10.2", null))
            : CalculatorWith();

        // Act / Assert
        Assert.Equal(expected, calculator.Calculate("ipconfig()").Text);
    }

    [Theory]
    [InlineData("ipconfig(1)", "Function 'ipconfig' expects no arguments")]
    [InlineData("ipconfig() + 1", "Can't add a text and a number")]
    [InlineData("today + ipconfig()", "Can't add a date and a text")]
    [InlineData("sqrt(ipconfig())", "Expected a number, not a text")]
    [InlineData("-ipconfig()", "A text can't be negative")]
    [InlineData("ipconfig = 2", "'ipconfig' is a function, so it can't be given a value")]
    public void IpConfig_IsAText_ThatCantBeCalculatedWith(string expression, string message)
    {
        // Arrange
        ExpressionCalculator calculator = CalculatorWith(WiFi);

        // Act
        var error = Assert.Throws<ExpressionException>(() => calculator.Calculate(expression));

        // Assert
        Assert.StartsWith(message, error.Message);
    }

    [Fact]
    public void IpConfig_KeptInAName_IsTheSameText()
    {
        // Arrange
        ExpressionCalculator calculator = CalculatorWith(WiFi);
        CalculationResult kept = calculator.Calculate("net = ipconfig()");

        // Act
        var names = new Dictionary<string, CalculatorValue> { ["net"] = kept.Value! };
        CalculationResult result = calculator.Calculate("net", names);

        // Assert
        Assert.Equal(kept.Text, result.Text);
    }

    [Fact]
    public void SystemReader_ReadsThisComputersAddresses_WithoutFailing()
    {
        // Act
        IReadOnlyList<NetworkAddressModel> addresses = new SystemNetworkAddressReader().Read();

        // Assert: whatever network the computer has, none of them is the loopback
        Assert.DoesNotContain(addresses, address => address.Address is "127.0.0.1" or "::1");
    }

    private sealed class FakeNetwork(IReadOnlyList<NetworkAddressModel> addresses) : INetworkAddressReader
    {
        public IReadOnlyList<NetworkAddressModel> Read() => addresses;
    }
}
