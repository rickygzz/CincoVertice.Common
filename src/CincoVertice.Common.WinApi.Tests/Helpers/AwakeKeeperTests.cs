using CincoVertice.Common.WinApi.Helpers;
using CincoVertice.Common.WinApi.Libs;
using Xunit;

namespace CincoVertice.Common.WinApi.Tests.Helpers;

/// <summary>
///     With Windows itself: what the thread needs is read back by setting it again, which returns the old.
/// </summary>
public class AwakeKeeperTests
{
    private const Kernel32.ExecutionState Awake =
        Kernel32.ExecutionState.ES_CONTINUOUS
        | Kernel32.ExecutionState.ES_SYSTEM_REQUIRED
        | Kernel32.ExecutionState.ES_DISPLAY_REQUIRED;

    // What this thread asks of Windows now; asking it again leaves it as it was
    private static Kernel32.ExecutionState Current()
    {
        Kernel32.ExecutionState state = Kernel32.SetThreadExecutionState(Kernel32.ExecutionState.ES_CONTINUOUS);
        Kernel32.SetThreadExecutionState(state);

        return state;
    }

    [Fact]
    public void TurnOn_KeepsTheComputerAwakeAndTheScreenOn_TurnOff_NoLonger()
    {
        // Arrange
        var keeper = new AwakeKeeper();

        try
        {
            // Act
            keeper.TurnOn();

            // Assert
            Assert.True(keeper.IsOn);
            Assert.Equal(Awake, Current());

            // And
            keeper.TurnOff();
            Assert.False(keeper.IsOn);
            Assert.Equal(Kernel32.ExecutionState.ES_CONTINUOUS, Current());
        }
        finally
        {
            keeper.TurnOff();
        }
    }

    [Fact]
    public void TurnOff_WhenOff_ChangesNothing()
    {
        // Arrange
        var keeper = new AwakeKeeper();

        // Act
        keeper.TurnOff();

        // Assert
        Assert.False(keeper.IsOn);
    }
}
