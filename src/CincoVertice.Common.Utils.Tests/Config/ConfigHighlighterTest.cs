using CincoVertice.Common.Utils.Config;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Config;

public class ConfigHighlighterTest
{
    private static List<string> Spans(string text) =>
        [.. new ConfigHighlighter().Find(text).Select(span => $"{span.Kind} {text.Substring(span.Start, span.Length)}")];

    [Fact]
    public void Find_ColorsCommentsKeysAndValues()
    {
        // Arrange
        const string text = "# top\nInterval = 5000\nMove = true\nName = Ctrl+Alt+1, app1\n";

        // Act & Assert
        Assert.Equal(
            [
                "Comment # top",
                "Key Interval",
                "Number 5000",
                "Key Move",
                "Keyword true",
                "Key Name",
                "Text Ctrl+Alt+1, app1",
            ],
            Spans(text));
    }

    [Theory]
    [InlineData("A = Ctrl+1 # note", "Text Ctrl+1", "Comment # note")]
    [InlineData("A = Ctrl+# # note", "Text Ctrl+#", "Comment # note")]
    [InlineData("A = Ctrl+1, app#1", "Text Ctrl+1, app#1")]
    [InlineData("A = 5 # five", "Number 5", "Comment # five")]
    public void Find_SplitsACommentFromTheValue(string text, params string[] value)
    {
        Assert.Equal(["Key A", .. value], Spans(text));
    }

    [Theory]
    [InlineData("")]
    [InlineData("not a line")]
    [InlineData("A = [\"never closed")]
    [InlineData("A =")]
    public void Find_BrokenText_DoesntThrow(string text)
    {
        Assert.NotNull(Spans(text));
    }

    [Fact]
    public void GetLine_GivesTheLineOfEachKey()
    {
        // Arrange
        ConfigLexer lexer = new("# top\nA = 1\n\nB = 2");

        // Act & Assert
        Assert.Equal(2, lexer.Items.GetLine(0));
        Assert.Equal(4, lexer.Items.GetLine(1));
        Assert.Equal(0, lexer.Items.GetLine(2));
    }
}
