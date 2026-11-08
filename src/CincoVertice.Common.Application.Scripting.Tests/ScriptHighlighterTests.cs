using CincoVertice.Common.Application.Scripting;
using CincoVertice.Common.Application.Scripting.Models;
using CincoVertice.Common.Utils.Highlighting;
using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class ScriptHighlighterTests
{
    private static List<string> Spans(string source) =>
        [.. new ScriptHighlighter().Find(source).Select(span => $"{span.Kind} {source.Substring(span.Start, span.Length)}")];

    [Fact]
    public void Find_ColorsEachKindOfToken()
    {
        // Arrange
        const string source = "# note\nif text.trim(name) != \"a\\\"b\" and size(1.5)\n  x = 2\nend";

        // Act & Assert
        Assert.Equal(
            [
                "Comment # note",
                "Keyword if",
                "Function text.trim",
                "Text \"a\\\"b\"",
                "Keyword and",
                "Function size",
                "Number 1.5",
                "Number 2",
                "Keyword end",
            ],
            Spans(source));
    }

    [Fact]
    public void Find_AMultiLineText_IsOneSpan()
    {
        // Arrange
        const string source = "a = \"\"\"\n  # not a comment\n  \"\"\"\nb = 1";

        // Act & Assert
        Assert.Equal(["Text \"\"\"\n  # not a comment\n  \"\"\"", "Number 1"], Spans(source));
    }

    [Theory]
    [InlineData("a = \"open\nb = 1", "Text \"open", "Number 1")]
    [InlineData("a = \"\"\"\nnever closed", "Text \"\"\"\nnever closed")]
    [InlineData("a = \"ends in \\", "Text \"ends in \\")]
    public void Find_ABrokenScript_StillColors(string source, params string[] expected)
    {
        Assert.Equal(expected, Spans(source));
    }

    [Theory]
    [InlineData("a $ b")]
    [InlineData("!")]
    [InlineData("")]
    public void Find_WhatIsntKnown_HasNoSpan(string source)
    {
        Assert.Empty(Spans(source.Replace("a", "_a").Replace("b", "_b")));
    }

    [Fact]
    public void Find_KeywordsAreTheLexersKeywords()
    {
        // Arrange: every token the lexer calls a keyword, and a name that's close to one
        const string source = "if elseif else end and or not true false IF ending";
        List<string> lexerKeywords = [.. ScriptLexer.Tokenize(source)
            .Where(token => token.Type == TokenType.Keyword)
            .Select(token => token.Value)];

        // Act
        List<string> highlighted = [.. new ScriptHighlighter().Find(source)
            .Where(span => span.Kind == CodeSpanKind.Keyword)
            .Select(span => source.Substring(span.Start, span.Length).ToLowerInvariant())];

        // Assert
        Assert.Equal(lexerKeywords, highlighted);
    }

    [Theory]
    [InlineData("x = 1\ny = (", 2)]
    [InlineData("x = 1\n\nif x\n", 4)]
    [InlineData("a = \"open", 1)]
    public void FindSyntaxError_GivesTheLine(string source, int line)
    {
        // Act
        ScriptException? error = ScriptEngine.FindSyntaxError(source);

        // Assert
        Assert.NotNull(error);
        Assert.Equal(line, error.Line);
    }

    [Fact]
    public void FindSyntaxError_AGoodScript_HasNone_AndNothingRuns()
    {
        // unknown.function would fail if it ran
        Assert.Null(ScriptEngine.FindSyntaxError("unknown.function()\nx = 1"));
    }
}
