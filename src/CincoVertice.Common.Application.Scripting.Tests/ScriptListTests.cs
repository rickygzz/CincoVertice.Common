using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class ScriptListTests
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
    public async Task RunAsync_FormAsksEverythingInOneDialog()
    {
        // Arrange
        _host.Clipboard = "records";
        _host.FormAnswers.Enqueue(["c# records", "learn.microsoft.com", "en"]);
        string script =
            "answers = dialog.form(\"Search Google\",\n"
            + "    [\"Search for:\", \"Only on site:\", \"Language:\"],\n"
            + "    [clipboard.gettext(), \"\", \"en\"])\n"
            + "\n"
            + "query = answers[1]\n"
            + "site = answers[2]\n"
            + "lang = answers[3]\n"
            + "if site != \"\"\n"
            + "    query = query + \" site:\" + site\n"
            + "end\n"
            + "shell.open(\"https://www.google.com/search?hl=\" + lang + \"&q=\" + text.urlencode(query))\n";

        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal(
            [
                "form: Search Google [Search for:=records | Only on site:= | Language:=en]",
                "open: https://www.google.com/search?hl=en&q=c%23%20records%20site%3Alearn.microsoft.com",
            ],
            calls);
    }

    [Fact]
    public async Task RunAsync_FormWithoutDefaults_StartsEveryBoxEmpty()
    {
        // Arrange
        _host.FormAnswers.Enqueue(["a", "b"]);

        // Act
        var calls = await RunAsync("dialog.message(dialog.form(\"Two\", [\"First:\", \"Second:\"])[2])");

        // Assert
        Assert.Equal(["form: Two [First:= | Second:=]", "say: b"], calls);
    }

    [Fact]
    public async Task RunAsync_FormDefaultThatIsAListIsAFixedList()
    {
        // Arrange
        _host.FormAnswers.Enqueue(["DEV-1", "Blue", "x"]);
        string script =
            "answers = dialog.form(\"PR\",\n"
            + "    [\"Task:\", \"Color:\", \"Note:\"],\n"
            + "    [\"DEV-700\", [\"Red\", \"Blue\", 3]])\n"
            + "dialog.message(answers[2])";

        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal(["form: PR [Task:=DEV-700 | Color:={Red, Blue, 3} | Note:=]", "say: Blue"], calls);
    }

    [Fact]
    public async Task RunAsync_FormListWithSelectedItem_PassesWhichToSelect()
    {
        // Arrange
        _host.FormAnswers.Enqueue(["Mobile"]);

        // Act
        var calls = await RunAsync(
            "app = \"Mobile\"\nanswers = dialog.form(\"PR\", [\"App:\"], [[[\"Finpro\", \"Mobile\", \"Life\"], app]])");

        // Assert
        Assert.Equal(["form: PR [App:={Finpro, Mobile, Life} Mobile]"], calls);
    }

    [Fact]
    public async Task RunAsync_WhenFormIsCancelled_StopsWithoutError()
    {
        // Arrange
        _host.FormAnswers.Enqueue(null);

        // Act
        var calls = await RunAsync("answers = dialog.form(\"T\", [\"A:\"])\ndialog.message(answers[1])");

        // Assert
        Assert.Equal(["form: T [A:=]"], calls);
    }

    [Theory]
    [InlineData("dialog.message([1, \"a\", true])", "say: [1, a, true]")]
    [InlineData("dialog.message([])", "say: []")]
    [InlineData("dialog.message([1, 2] + [3])", "say: [1, 2, 3]")]
    [InlineData("dialog.message(\"Items: \" + [1, 2])", "say: Items: [1, 2]")]
    [InlineData("dialog.message(list.length([1, 2, 3]) + text.length(\"ab\"))", "say: 5")]
    [InlineData("dialog.message(list.contains([\"a\", \"b\"], \"b\"))", "say: true")]
    [InlineData("dialog.message(list.contains([\"a\", \"b\"], \"B\"))", "say: false")]
    [InlineData("dialog.message([1, [2, 3]] == [1, [2, 3]])", "say: true")]
    [InlineData("dialog.message([1, 2] != [1, 2, 3])", "say: true")]
    [InlineData("dialog.message([[\"x\", \"y\"]][1][2])", "say: y")]
    [InlineData("items = [\n    \"a\",\n    \"b\"\n]\ndialog.message(items[1 + 1])", "say: b")]
    public async Task RunAsync_EvaluatesLists(string script, string expected)
    {
        // Act
        var calls = await RunAsync(script);

        // Assert
        Assert.Equal([expected], calls);
    }

    [Theory]
    [InlineData("x = [1, 2]\ndialog.message(x[0])", 2, 18, "There is no item 0: the list has 2 items, and the first")]
    [InlineData("x = [1]\ndialog.message(x[2])", 2, 18, "the list has 1 item")]
    [InlineData("x = [1, 2]\ndialog.message(x[1.5])", 2, 18, "must be a whole number, not 1.5")]
    [InlineData("x = [1, 2]\ndialog.message(x[\"1\"])", 2, 18, "must be a whole number, not 1")]
    [InlineData("x = \"abc\"\ndialog.message(x[1])", 2, 17, "Only a list has items")]
    [InlineData("x = [1]\nx[1] = 2", 2, 6, "A list's items can't be changed")]
    [InlineData("x = [1, 2", 1, 10, "Missing ']' to close the list")]
    [InlineData("dialog.message([1] + 2)", 1, 20, "'+' needs numbers, not a list")]
    [InlineData("dialog.message([1] < [2])", 1, 20, "compares two numbers or two texts, not a list and a list")]
    [InlineData("dialog.form(\"T\", \"A:\")", 1, 1, "dialog.form(): argument 2 must be a list, not a text")]
    [InlineData("dialog.form(\"T\", [])", 1, 1, "dialog.form(): the list of prompts is empty")]
    [InlineData("dialog.form(\"T\", [\"A:\"], [\"1\", \"2\"])", 1, 1, "there are 2 defaults for 1 prompts")]
    [InlineData("dialog.form(\"T\", [\"Color:\"], [[]])", 1, 1, "the list of choices for 'Color:' is empty")]
    [InlineData("dialog.form(\"T\", [\"Color:\"], [[[], \"x\"]])", 1, 1, "the list of choices for 'Color:' is empty")]
    [InlineData("dialog.form(\"T\", [\"C:\"], [[[\"a\"], \"a\", \"b\"]])", 1, 1, "must be [items] or [[items], sel")]
    [InlineData("dialog.form(\"T\", [\"C:\"], [[[\"a\"], [\"a\"]]])", 1, 1, "must be [items] or [[items], sel")]
    public async Task RunAsync_ReportsListErrorsWithPosition(string script, int line, int column, string message)
    {
        // Act
        var error = await RunWithErrorAsync(script);

        // Assert
        Assert.Equal((line, column), (error.Line, error.Column));
        Assert.Contains(message, error.Message);
    }
}
