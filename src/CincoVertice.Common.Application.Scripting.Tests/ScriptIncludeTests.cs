using CincoVertice.Common.Application.Scripting.Models;
using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class ScriptIncludeTests
{
    private readonly FakeScriptHost _host = new();

    // The scripts that script.include can read, by name
    private readonly Dictionary<string, string> _scripts = new(StringComparer.OrdinalIgnoreCase);

    private ScriptRunOptions Options(string argument = "") => new()
    {
        Argument = argument,
        ReadInclude = name => _scripts.TryGetValue(name, out string? source)
            ? source
            : throw new InvalidOperationException($"There is no script {name}.mts"),
    };

    private async Task<List<string>> RunAsync(string script, ScriptRunOptions? options = null)
    {
        await new ScriptEngine().RunAsync(script, _host, options ?? Options());

        return _host.Calls;
    }

    private async Task<ScriptException> RunWithErrorAsync(string script, ScriptRunOptions? options = null) =>
        await Assert.ThrowsAsync<ScriptException>(
            () => new ScriptEngine().RunAsync(script, _host, options ?? Options()));

    [Theory]
    [InlineData("app1", "dialog.message(script.argument())", "say: app1")]
    [InlineData("", "dialog.message(\"[\" + script.argument() + \"]\")", "say: []")]
    [InlineData("", "dialog.message(script.argument(\"app1\"))", "say: app1")]
    [InlineData("app2", "dialog.message(script.argument(\"app1\"))", "say: app2")]
    public async Task RunAsync_ArgumentIsTheValueFromTheHotkey(string argument, string script, string expected)
    {
        // Act
        var calls = await RunAsync(script, Options(argument));

        // Assert
        Assert.Equal([expected], calls);
    }

    [Fact]
    public async Task RunAsync_IncludedScriptSharesTheVariables()
    {
        // Arrange
        _scripts["_Greeting"] = "message = \"Hello \" + name\nshown = true";

        // Act
        var calls = await RunAsync(
            "name = \"Ricky\"\nscript.include(\"_Greeting\")\nif shown\n    dialog.message(message)\nend");

        // Assert
        Assert.Equal(["say: Hello Ricky"], calls);
    }

    [Fact]
    public async Task RunAsync_IncludedScriptCanIncludeAnother()
    {
        // Arrange
        _scripts["A"] = "script.include(\"B\")\ndialog.message(\"a\")";
        _scripts["B"] = "dialog.message(\"b\")";

        // Act
        var calls = await RunAsync("script.include(\"A\")\nscript.include(\"b\")");

        // Assert
        Assert.Equal(["say: b", "say: a", "say: b"], calls);
    }

    [Fact]
    public async Task RunAsync_StopInIncludedScriptEndsEverything()
    {
        // Arrange
        _scripts["Stopper"] = "script.stop()";

        // Act
        var calls = await RunAsync("script.include(\"Stopper\")\ndialog.message(\"after\")");

        // Assert
        Assert.Empty(calls);
    }

    [Theory]
    [InlineData("x = 1\n  y = nmae", "script.include(): in Lib, line 2, column 7: Unknown variable 'nmae'")]
    [InlineData("if true", "script.include(): in Lib, line 1, column 8: Missing 'end' for the 'if' on line 1")]
    [InlineData("script.include(\"Lib\")", "levels deep: does Lib include itself?")]
    public async Task RunAsync_ReportsErrorsOfIncludedScriptAtTheInclude(string included, string message)
    {
        // Arrange
        _scripts["Lib"] = included;

        // Act
        var error = await RunWithErrorAsync("x = 0\n  script.include(\"Lib\")");

        // Assert
        Assert.Equal((2, 3), (error.Line, error.Column));
        Assert.Contains(message, error.Message);
    }

    [Fact]
    public async Task RunAsync_WhenIncludedScriptIsMissing_SaysSo()
    {
        // Act
        var error = await RunWithErrorAsync("script.include(\"Missing\")");

        // Assert
        Assert.Equal("script.include(): There is no script Missing.mts", error.Message);
    }

    [Fact]
    public async Task RunAsync_WithoutIncludes_ReportsThatScriptsCantInclude()
    {
        // Act
        var error = await RunWithErrorAsync("script.include(\"Lib\")", new ScriptRunOptions());

        // Assert
        Assert.Equal("script.include(): this script can't include other scripts", error.Message);
    }
}
