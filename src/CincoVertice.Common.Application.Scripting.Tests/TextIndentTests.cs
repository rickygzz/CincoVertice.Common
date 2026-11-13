using CincoVertice.Common.Application.Scripting.Helpers;
using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class TextIndentTests
{
    [Theory]
    [InlineData(new[] { "    a", "    b" }, "    ", "a\nb")]
    [InlineData(new[] { "    a", "      b" }, "    ", "a\n  b")]
    [InlineData(new[] { "a", "b" }, "", "a\nb")]
    [InlineData(new[] { "    a", "", "  ", "    b" }, "    ", "a\n\n\nb")]
    [InlineData(new[] { "\ta", "\t\tb" }, "\t", "a\n\tb")]
    [InlineData(new string[0], "    ", "")]
    public void TryRemove_RemovesTheIndentFromEveryLine(string[] lines, string indent, string expected)
    {
        // Act
        bool ok = TextIndent.TryRemove(lines, indent, out string text, out int badLine);

        // Assert
        Assert.True(ok);
        Assert.Equal((expected, -1), (text, badLine));
    }

    [Theory]
    [InlineData(new[] { "    a", "  b", " c" }, "    ", 1)]
    [InlineData(new[] { "a" }, "  ", 0)]
    [InlineData(new[] { "\ta" }, "    ", 0)]
    [InlineData(new[] { "    a" }, "\t", 0)]
    public void TryRemove_WhenALineStartsLeftOfTheIndent_SaysWhichFirst(string[] lines, string indent, int expected)
    {
        // Act
        bool ok = TextIndent.TryRemove(lines, indent, out string text, out int badLine);

        // Assert
        Assert.False(ok);
        Assert.Equal((string.Empty, expected), (text, badLine));
    }
}
