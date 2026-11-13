using CincoVertice.Common.WinApi.Helpers;
using Xunit;

namespace CincoVertice.Common.WinApi.Tests.Helpers;

/// <summary>
///     Only what can be tested without starting the simulator, which would move the real mouse.
/// </summary>
public class MouseMoveSimulatorTests
{
    [Fact]
    public void Settings_StartAsTheDefaults()
    {
        // Arrange
        using var simulator = new MouseMoveSimulator();

        // Assert
        Assert.Same(MouseMoveSettings.Default, simulator.Settings);
        Assert.False(simulator.IsRunning);
    }

    [Fact]
    public void Settings_AreReplacedAsAWhole()
    {
        // Arrange
        using var simulator = new MouseMoveSimulator();
        var settings = new MouseMoveSettings(1000, 0, 50, 50, false);

        // Act
        simulator.Settings = settings;

        // Assert
        Assert.Same(settings, simulator.Settings);
    }

    [Theory]
    [InlineData(0, 20, 20, 100)]
    [InlineData(-1, 20, 20, 100)]
    [InlineData(5000, -1, 20, 100)]
    [InlineData(5000, 20, 80, 40)]
    public void Settings_ThatCantBeUsed_AreRejectedAndTheOldOnesKept(int interval, int idleTicks, int min, int max)
    {
        // Arrange
        using var simulator = new MouseMoveSimulator();

        // Act and assert
        Assert.Throws<ArgumentException>(
            () => simulator.Settings = new MouseMoveSettings(interval, idleTicks, min, max, true));
        Assert.Same(MouseMoveSettings.Default, simulator.Settings);
    }
}
