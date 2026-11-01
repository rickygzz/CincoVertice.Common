using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class ScriptEngineTests
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
    public async Task RunAsync_AsksAndOpensSearchPage()
    {
        // Arrange
        _host.Answers.Enqueue("c# records");
        string script =
            "# Search Google\n"
            + "query = dialog.ask(\"Search Google for:\")\n"
            + "if query != \"\"\n"
            + "    shell.open(\"https://www.google.com/search?q=\" + text.urlencode(query))\n"
            + "end\n";

        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal(
            ["ask: Search Google for: []", "open: https://www.google.com/search?q=c%23%20records"],
            calls);
    }

    [Fact]
    public async Task RunAsync_WhenAskIsCancelled_StopsWithoutError()
    {
        // Arrange
        _host.Answers.Enqueue(null);

        // Act
        var calls = await RunAsync("name = dialog.ask(\"Name?\", \"Ricky\")\ndialog.message(\"Hello \" + name)");

        // Assert
        Assert.Equal(["ask: Name? [Ricky]"], calls);
    }

    [Theory]
    [InlineData(5, "say: small")]
    [InlineData(15, "say: medium")]
    [InlineData(25, "say: large")]
    public async Task RunAsync_RunsFirstBranchWhoseConditionIsTrue(int size, string expected)
    {
        // Arrange
        string script =
            $"size = {size}\n"
            + "if size < 10\n"
            + "    dialog.message(\"small\")\n"
            + "elseif size >= 10 and size < 20\n"
            + "    dialog.message(\"medium\")\n"
            + "else\n"
            + "    dialog.message(\"large\")\n"
            + "end";

        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal([expected], calls);
    }

    [Theory]
    [InlineData("dialog.message(1 + 2 * 3)", "say: 7")]
    [InlineData("dialog.message((1 + 2) * 3)", "say: 9")]
    [InlineData("dialog.message(7 % 3)", "say: 1")]
    [InlineData("dialog.message(-2 + 5)", "say: 3")]
    [InlineData("dialog.message(10 / 4)", "say: 2.5")]
    [InlineData("dialog.message(\"Total: \" + 2 * 3)", "say: Total: 6")]
    [InlineData("dialog.message(\"a\" == \"a\" and not (1 == 2))", "say: true")]
    [InlineData("dialog.message(\"1\" == 1)", "say: false")]
    [InlineData("dialog.message(false or true)", "say: true")]
    [InlineData("dialog.message(\"abc\" < \"abd\")", "say: true")]
    [InlineData("dialog.message(\"say \\\"hi\\\"\\tnow\")", "say: say \"hi\"\tnow")]
    public async Task RunAsync_EvaluatesExpressions(string script, string expected)
    {
        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal([expected], calls);
    }

    [Theory]
    [InlineData("dialog.message(text.length(\"hello\"))", "say: 5")]
    [InlineData("dialog.message(text.upper(\"ab\") + text.lower(\"CD\"))", "say: ABcd")]
    [InlineData("dialog.message(text.trim(\"  x  \"))", "say: x")]
    [InlineData("dialog.message(text.replace(\"a-b-c\", \"-\", \"+\"))", "say: a+b+c")]
    [InlineData("dialog.message(text.contains(\"Hello\", \"hello\"))", "say: true")]
    [InlineData("dialog.message(number(\" 12.5 \") * 2)", "say: 25")]
    [InlineData("dialog.message(text(3) + text(true))", "say: 3true")]
    [InlineData("DIALOG.MESSAGE(Text.Length(\"ab\"))", "say: 2")]
    public async Task RunAsync_CallsTextFunctions(string script, string expected)
    {
        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal([expected], calls);
    }

    [Fact]
    public async Task RunAsync_CallsDesktopFunctions()
    {
        // Arrange
        _host.Clipboard = "copied";
        _host.Confirmations.Enqueue(true);
        string script =
            "if dialog.confirm(\"Go?\")\n"
            + "    shell.run(\"notepad.exe\", \"a.txt\")\n"
            + "    clipboard.settext(clipboard.gettext() + \"!\")\n"
            + "    script.wait(250)\n"
            + "    keyboard.type(\"Hello\")\n"
            + "    keyboard.press(\"Ctrl+V\")\n"
            + "    keyboard.press(\"Tab\", 3)\n"
            + "    tray.notify(\"Done\")\n"
            + "end";

        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal(
            [
                "confirm: Go?",
                "run: notepad.exe [a.txt]",
                "copy: copied!",
                "wait: 250",
                "type: Hello",
                "press: Ctrl+V [1]",
                "press: Tab [3]",
                "notify: Done",
            ],
            calls);
    }

    [Fact]
    public async Task RunAsync_ReadsActiveWindowTitle()
    {
        // Arrange
        _host.ActiveWindowTitle = "Inbox - Outlook";

        // Act
        var calls = await RunAsync(
            "if text.contains(window.title(), \"outlook\")\n    dialog.message(window.title())\nend");

        // Assert
        Assert.Equal(["say: Inbox - Outlook"], calls);
    }

    [Theory]
    [InlineData("Bold text", new[] { "paste: Bold text" })]
    [InlineData("", new string[0])]
    public async Task RunAsync_PasteAsTextPastesTheClipboardText(string clipboard, string[] expected)
    {
        // Arrange
        _host.Clipboard = clipboard;

        // Act
        var calls = await RunAsync("clipboard.pasteastext()");

        // Assert
        Assert.Equal(expected, calls);
    }

    [Fact]
    public async Task RunAsync_ClearEmptiesTheClipboard()
    {
        // Act
        var calls = await RunAsync("clipboard.clear()");

        // Assert
        Assert.Equal(["copy: "], calls);
    }

    [Theory]
    [InlineData("keyboard.press(\"Foo\")", "keyboard.press(): 'Foo' is not a key.")]
    [InlineData("keyboard.press(\"Tab\", 0)", "keyboard.press(): the times must be a whole number from 1 to 100")]
    [InlineData("keyboard.press(\"Tab\", 1.5)", "the times must be a whole number from 1 to 100, not 1.5")]
    [InlineData("keyboard.press(\"Tab\", 101)", "the times must be a whole number from 1 to 100, not 101")]
    public async Task RunAsync_ReportsPressErrors(string script, string message)
    {
        // Act
        var error = await RunWithErrorAsync(script);

        // Assert
        Assert.Contains(message, error.Message);
    }

    [Fact]
    public async Task RunAsync_StopEndsScriptWithoutError()
    {
        // Act
        var calls = await RunAsync("dialog.message(\"a\")\nscript.stop()\ndialog.message(\"b\")");

        // Assert
        Assert.Equal(["say: a"], calls);
    }

    [Fact]
    public async Task RunAsync_CallsSpanningLinesInsideParentheses()
    {
        // Act
        var calls = await RunAsync("dialog.message(\n    \"a\" +\n    \"b\"\n)\r\n\r\n# done\r\n");

        // Assert
        Assert.Equal(["say: ab"], calls);
    }

    [Fact]
    public async Task RunAsync_WhenScriptHasSyntaxError_RunsNothing()
    {
        // Act
        var error = await RunWithErrorAsync("dialog.message(\"first\")\nif x = 1\n    dialog.message(\"x\")\nend");

        // Assert
        Assert.Empty(_host.Calls);
        Assert.Equal(2, error.Line);
        Assert.Contains("Use == to compare", error.Message);
    }

    [Theory]
    [InlineData("dialog.message(\"a)", 1, 16, "not closed")]
    [InlineData("x = 1 !", 1, 7, "use 'not'")]
    [InlineData("dialog.message('a')", 1, 16, "double quotes")]
    [InlineData("x == 1", 1, 1, "A line must assign")]
    [InlineData("if true\n    dialog.message(1)", 2, 22, "Missing 'end' for the 'if' on line 1")]
    [InlineData("end", 1, 1, "'end' without an 'if'")]
    [InlineData("dialog.message(1, 2", 1, 20, "Missing ')'")]
    [InlineData("x = ", 1, 5, "Expected a value")]
    [InlineData("x = 1 < 2 < 3", 1, 11, "two values at a time")]
    [InlineData("if true then\nend", 1, 9, "end of the line")]
    public async Task RunAsync_ReportsSyntaxErrorsWithPosition(string script, int line, int column, string message)
    {
        // Act
        var error = await RunWithErrorAsync(script);

        // Assert
        Assert.Equal((line, column), (error.Line, error.Column));
        Assert.Contains(message, error.Message);
    }

    [Theory]
    [InlineData("dialog.message(nmae)", 1, 16, "Unknown variable 'nmae'")]
    [InlineData("x = clipboard.gettext\n", 1, 5, "write clipboard.gettext()")]
    [InlineData("foo()", 1, 1, "Unknown function 'foo'")]
    [InlineData("dialog.message()", 1, 1, "dialog.message() takes 1 argument, not 0")]
    [InlineData("dialog.message(1 / 0)", 1, 18, "Division by zero")]
    [InlineData("dialog.message(\"a\" - 1)", 1, 20, "'-' needs numbers, not a text")]
    [InlineData("if \"yes\"\nend", 1, 4, "must be true or false, not a text")]
    [InlineData("x = dialog.message(\"a\")", 1, 5, "dialog.message() returns no value")]
    [InlineData("dialog.message(1 < \"a\")", 1, 18, "compares two numbers or two texts")]
    [InlineData("script.wait(\"a\")", 1, 1, "script.wait(): argument 1 must be a number")]
    [InlineData("script.wait(-1)", 1, 1, "script.wait(): the time must be")]
    [InlineData("dialog.message(number(\"abc\"))", 1, 16, "number(): 'abc' is not a number")]
    [InlineData("\n\n  dialog.message(true and 1)", 3, 27, "The value after 'and' must be true or false")]
    public async Task RunAsync_ReportsRunTimeErrorsWithPosition(string script, int line, int column, string message)
    {
        // Act
        var error = await RunWithErrorAsync(script);

        // Assert
        Assert.Equal((line, column), (error.Line, error.Column));
        Assert.Contains(message, error.Message);
    }

    [Theory]
    [InlineData("copy(\"x\")", "Unknown function 'copy'")]
    [InlineData("say(\"x\")", "Unknown function 'say'")]
    [InlineData("x = len(\"ab\")", "Unknown function 'len'")]
    [InlineData("x = clipboard", "Unknown variable 'clipboard'")]
    [InlineData(
        "clipboard.copy(\"x\")",
        "Unknown function 'clipboard.copy': clipboard has clear, gettext, pasteastext, settext")]
    [InlineData("files.read(\"a.txt\")", "Unknown function 'files.read': there is no module 'files'")]
    [InlineData("my.name = 1", "A variable name can't contain '.': 'my.name' looks like a function")]
    [InlineData("x = text.length([1, 2])", "text.length(): argument 1 is a list: use list.length()")]
    [InlineData("x = text.contains([1], 1)", "text.contains(): argument 1 is a list: use list.contains()")]
    [InlineData("x = list.length(\"ab\")", "list.length(): argument 1 must be a list, not a text")]
    public async Task RunAsync_ReportsModuleErrorsWithHints(string script, string message)
    {
        // Act
        var error = await RunWithErrorAsync(script);

        // Assert
        Assert.Equal(message, error.Message);
    }

    [Fact]
    public async Task RunAsync_CallsModuleFunctionsCaseInsensitively()
    {
        // Arrange
        _host.Clipboard = "abc";

        // Act
        var calls = await RunAsync("Clipboard.SetText(TEXT.UPPER(clipboard.GETTEXT()))");

        // Assert
        Assert.Equal(["copy: ABC"], calls);
    }

    [Fact]
    public async Task RunAsync_ReportsHostFailureAsErrorAtCall()
    {
        // Arrange
        _host.OpenFailure = new InvalidOperationException("The system cannot find the file specified.");

        // Act
        var error = await RunWithErrorAsync("x = 1\n  shell.open(\"missing.txt\")");

        // Assert
        Assert.Equal((2, 3), (error.Line, error.Column));
        Assert.Equal("shell.open(): The system cannot find the file specified.", error.Message);
        Assert.Equal("Line 2, column 3: shell.open(): The system cannot find the file specified.", error.Description);
    }
}
