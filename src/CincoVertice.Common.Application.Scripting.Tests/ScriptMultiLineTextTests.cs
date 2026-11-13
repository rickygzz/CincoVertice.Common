using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class ScriptMultiLineTextTests
{
    private readonly FakeScriptHost _host = new();

    private async Task<List<string>> RunAsync(string script)
    {
        await new ScriptEngine().RunAsync(script, _host);

        return _host.Calls;
    }

    private async Task<ScriptException> RunWithErrorAsync(string script) =>
        await Assert.ThrowsAsync<ScriptException>(() => new ScriptEngine().RunAsync(script, _host));

    [Fact]
    public async Task RunAsync_TakesLinesAsTheyAreWithoutEscapes()
    {
        // Arrange
        string script =
            "template = \"\"\"\n"
            + "## 📝 Description\n"
            + "<!-- Say \"why\", e.g. C:\\temp -->\n"
            + "\n"
            + "- [ ] # not a comment\n"
            + "\"\"\"\n"
            + "clipboard.paste(template)\n";

        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal(
            ["paste: ## 📝 Description\n<!-- Say \"why\", e.g. C:\\temp -->\n\n- [ ] # not a comment"],
            calls);
    }

    [Fact]
    public async Task RunAsync_RemovesTheIndentOfTheClosingQuotes()
    {
        // Arrange
        string script =
            "if true\n"
            + "    clipboard.paste(\"\"\"\n"
            + "        First\n"
            + "          Indented\n"
            + "\n"
            + "        Last\n"
            + "        \"\"\")\n"
            + "end\n"
            + "dialog.message(\"after\")";

        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal(["paste: First\n  Indented\n\nLast", "say: after"], calls);
    }

    [Fact]
    public async Task RunAsync_AcceptsCrlfLines()
    {
        // Act
        var calls = await RunAsync("x = \"\"\"\r\na\r\nb\r\n\"\"\"\r\ndialog.message(x)\r\n");

        // Assert
        Assert.Equal(["say: a\nb"], calls);
    }

    [Theory]
    [InlineData("x = \"\"\"\na\nb", 1, 5, "The text is not closed: add a line starting with \"\"\"")]
    [InlineData("x = \"\"\"a\n\"\"\"", 1, 8, "Start the text on the line after \"\"\"")]
    [InlineData("x = \"\"\"\n  a\nb\n  \"\"\"", 3, 1, "This line starts left of the closing \"\"\"")]
    public async Task RunAsync_ReportsMultiLineTextErrors(string script, int line, int column, string message)
    {
        // Act
        var error = await RunWithErrorAsync(script);

        // Assert
        Assert.Equal((line, column), (error.Line, error.Column));
        Assert.Contains(message, error.Message);
    }

    [Fact]
    public async Task RunAsync_PasteOfEmptyTextDoesNothing()
    {
        // Act
        var calls = await RunAsync("clipboard.paste(\"\")");

        // Assert
        Assert.Empty(calls);
    }
}
