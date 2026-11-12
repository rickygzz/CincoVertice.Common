using CincoVertice.Common.Application.Scripting.Models;
using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

/// <summary>
///     system.waitidle(cpu, disk, seconds, minutes), which measures the load 5 seconds at a time.
/// </summary>
public class ScriptWaitIdleTests
{
    // Starts the program only when the computer got quiet, as in the user guide
    private const string StartLater =
        "if system.waitidle(15, 20, 10, 1)\n"
        + "    shell.run(\"app.exe\")\n"
        + "else\n"
        + "    tray.notify(\"Still busy\")\n"
        + "end\n";

    private static readonly SystemLoadModel _quiet = new(5, 5);
    private static readonly SystemLoadModel _busyCpu = new(80, 5);
    private static readonly SystemLoadModel _busyDisk = new(5, 90);

    private readonly FakeScriptHost _host = new();

    private async Task<List<string>> RunAsync(string script)
    {
        await new ScriptEngine().RunAsync(script, _host);

        return _host.Calls;
    }

    private async Task<ScriptException> RunWithErrorAsync(string script) =>
        await Assert.ThrowsAsync<ScriptException>(() => new ScriptEngine().RunAsync(script, _host));

    [Fact]
    public async Task WaitIdle_QuietFromTheStart_IsTrueAfterTheQuietSeconds()
    {
        // Act
        var calls = await RunAsync(StartLater);

        // Assert: 10 seconds is 2 measures of 5
        Assert.Equal(["run: app.exe []"], calls);
        Assert.Equal(2, _host.LoadMeasures);
    }

    [Fact]
    public async Task WaitIdle_ABusyMeasure_StartsTheQuietTimeAgain()
    {
        // Arrange
        _host.Loads.Enqueue(_busyCpu);
        _host.Loads.Enqueue(_quiet);
        _host.Loads.Enqueue(_busyCpu);
        _host.Loads.Enqueue(_quiet);
        _host.Loads.Enqueue(_quiet);

        // Act
        var calls = await RunAsync(StartLater);

        // Assert
        Assert.Equal(["run: app.exe []"], calls);
        Assert.Equal(5, _host.LoadMeasures);
    }

    [Fact]
    public async Task WaitIdle_ABusyDisk_IsBusy()
    {
        // Arrange
        _host.Loads.Enqueue(_busyDisk);

        // Act
        await RunAsync(StartLater);

        // Assert
        Assert.Equal(3, _host.LoadMeasures);
    }

    [Fact]
    public async Task WaitIdle_AtTheLimits_IsQuiet()
    {
        // Arrange
        _host.Loads.Enqueue(new SystemLoadModel(15, 20));
        _host.Loads.Enqueue(new SystemLoadModel(15, 20));

        // Act
        await RunAsync(StartLater);

        // Assert
        Assert.Equal(2, _host.LoadMeasures);
    }

    [Fact]
    public async Task WaitIdle_NeverQuiet_IsFalseWhenTheMinutesAreOver()
    {
        // Arrange: a minute is 12 measures
        for (int i = 0; i < 20; i++)
        {
            _host.Loads.Enqueue(_busyCpu);
        }

        // Act
        var calls = await RunAsync(StartLater);

        // Assert
        Assert.Equal(["notify: Still busy"], calls);
        Assert.Equal(12, _host.LoadMeasures);
    }

    [Fact]
    public async Task WaitIdle_QuietSecondsNotAWholeMeasure_RoundUp()
    {
        // Act: 6 seconds needs 2 measures of 5
        await RunAsync("system.waitidle(15, 20, 6, 1)");

        // Assert
        Assert.Equal(2, _host.LoadMeasures);
    }

    [Theory]
    [InlineData("system.waitidle(0, 20, 60, 20)", "system.waitidle(): the CPU percent must be a whole number from 1 to 100, not 0")]
    [InlineData("system.waitidle(15, 101, 60, 20)", "system.waitidle(): the disk percent must be a whole number from 1 to 100, not 101")]
    [InlineData("system.waitidle(15, 20, 0.5, 20)", "system.waitidle(): the quiet seconds must be a whole number from 1 to 3600, not 0.5")]
    [InlineData("system.waitidle(15, 20, 60, 121)", "system.waitidle(): the minutes must be a whole number from 1 to 120, not 121")]
    [InlineData("system.waitidle(15, 20, 120, 1)", "system.waitidle(): the quiet seconds (120) are longer than the minutes (1) to wait")]
    [InlineData("system.waitidle(15, 20, 60)", "system.waitidle()")]
    public async Task WaitIdle_BadArguments_AreErrors_BeforeWaiting(string script, string message)
    {
        // Act
        var error = await RunWithErrorAsync(script);

        // Assert
        Assert.StartsWith(message, error.Message);
        Assert.Equal(0, _host.LoadMeasures);
    }
}
