using System.Diagnostics;
using CincoVertice.Common.WinApi.Helpers;
using Xunit;

namespace CincoVertice.Common.WinApi.Tests.Helpers;

/// <summary>
///     These read the real counters of this computer, so they check only what holds whatever it's doing.
/// </summary>
public class SystemLoadMeterTests
{
    [Fact]
    public async Task Read_GivesPercents()
    {
        // Arrange
        using var meter = new SystemLoadMeter();
        meter.Start();
        await Task.Delay(300, TestContext.Current.CancellationToken);

        // Act
        var (cpu, disk) = meter.Read();

        // Assert
        Assert.InRange(cpu, 0, 100);
        Assert.InRange(disk, 0, 100);
    }

    [Fact]
    public void Read_WhileAProcessorIsKeptBusy_SeesSomeUse()
    {
        // Arrange
        using var meter = new SystemLoadMeter();
        meter.Start();
        var watch = Stopwatch.StartNew();

        // Act: one processor of all of them busy, so at least a few percent
        while (watch.ElapsedMilliseconds < 500)
        {
            Thread.SpinWait(1000);
        }

        var (cpu, _) = meter.Read();

        // Assert
        Assert.True(cpu > 0, $"CPU was {cpu}%");
    }
}
