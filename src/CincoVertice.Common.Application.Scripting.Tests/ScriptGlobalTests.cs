using CincoVertice.Common.Application.Scripting.Services;
using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class ScriptGlobalTests
{
    private readonly FakeScriptHost _host = new();
    private readonly ScriptEngine _engine = new(new MemoryVariableStore());

    private async Task<List<string>> RunAsync(string script)
    {
        await _engine.RunAsync(script, _host);

        return _host.Calls;
    }

    private async Task<ScriptException> RunWithErrorAsync(string script) =>
        await Assert.ThrowsAsync<ScriptException>(() => _engine.RunAsync(script, _host));

    [Fact]
    public async Task RunAsync_KeepsGlobalsBetweenRuns()
    {
        // Arrange
        const string counter =
            "count = global.get(\"count\", 0) + 1\n"
            + "global.set(\"count\", count)\n"
            + "dialog.message(count)";

        // Act
        await RunAsync(counter);
        await RunAsync(counter);
        var calls = await RunAsync(counter);

        // Assert
        Assert.Equal(["say: 1", "say: 2", "say: 3"], calls);
    }

    [Theory]
    [InlineData("global.set(\"x\", 2.5)", "2.5")]
    [InlineData("global.set(\"x\", \"Ricky\")", "Ricky")]
    [InlineData("global.set(\"x\", true)", "true")]
    [InlineData("global.set(\"x\", [1, \"a\", [true]])", "[1, a, [true]]")]
    [InlineData("global.set(\"Name\", 1)\nglobal.set(\"NAME\", 2)", "2")]
    public async Task RunAsync_GetReturnsWhatSetStored(string set, string expected)
    {
        // Act
        var calls = await RunAsync(set + "\ndialog.message(global.get(\"name\", global.get(\"x\", 0)))");

        // Assert
        Assert.Equal([$"say: {expected}"], calls);
    }

    [Fact]
    public async Task RunAsync_HasRemoveAndNames()
    {
        // Arrange
        string script =
            "global.set(\"b\", 1)\n"
            + "global.set(\"a\", 2)\n"
            + "dialog.message(global.names())\n"
            + "global.remove(\"b\")\n"
            + "global.remove(\"never set\")\n"
            + "dialog.message(global.has(\"a\"))\n"
            + "dialog.message(global.has(\"B\"))";

        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal(["say: [a, b]", "say: true", "say: false"], calls);
    }

    [Theory]
    [InlineData("x = global.get(\"missing\")", "global.get(): there is no global variable 'missing'")]
    [InlineData("global.set(\" \", 1)", "global.set(): the name can't be empty")]
    [InlineData("global.set(1, 1)", "global.set(): the name must be a text, not a number")]
    [InlineData("global.set(\"x\", dialog.message(\"a\"))", "dialog.message() returns no value")]
    public async Task RunAsync_ReportsGlobalErrors(string script, string message)
    {
        // Act
        var error = await RunWithErrorAsync(script);

        // Assert
        Assert.Contains(message, error.Message);
    }
}
