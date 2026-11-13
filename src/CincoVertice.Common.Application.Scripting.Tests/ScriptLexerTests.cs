using CincoVertice.Common.Application.Scripting.Models;
using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class ScriptLexerTests
{
    /// <summary>
    ///     The tokens as "Type value@line:column", joined with " | "; a token without a value as "Type@line:column".
    ///     New lines and tabs in a value are written \n and \t.
    /// </summary>
    private static string Tokenize(string source) => string.Join(" | ", ScriptLexer.Tokenize(source).Select(t =>
        t.Value.Length == 0
            ? $"{t.Type}@{t.Line}:{t.Column}"
            : $"{t.Type} {t.Value.Replace("\n", "\\n").Replace("\t", "\\t")}@{t.Line}:{t.Column}"));

    [Fact]
    public void Tokenize_EndsWithNewLineAndEndOfScript()
    {
        Assert.Equal(
            "Identifier x@1:1 | Operator =@1:3 | Number 1@1:5 | NewLine@1:6 | EndOfScript@1:6",
            Tokenize("x = 1"));
    }

    [Fact]
    public void Tokenize_EmptyScript_IsOnlyEndOfScript()
    {
        Assert.Equal("EndOfScript@1:1", Tokenize(""));
    }

    [Fact]
    public void Tokenize_ReadsNumbersWithAndWithoutLeadingDigit()
    {
        Assert.Equal(
            "Identifier x@1:1 | Operator =@1:3 | Number 2.5@1:5 | Operator +@1:9 | Number .5@1:11 | NewLine@1:13"
            + " | EndOfScript@1:13",
            Tokenize("x = 2.5 + .5"));
    }

    [Fact]
    public void Tokenize_ReadsEveryOperatorLongestFirst()
    {
        Assert.Equal(
            "Number 1@1:1 | Operator <@1:3 | Number 2@1:5 | Operator <=@1:7 | Number 3@1:10 | Operator !=@1:12"
            + " | Number 4@1:15 | Operator ==@1:17 | Number 5@1:20 | Operator %@1:22 | Number 6@1:24 | NewLine@1:25"
            + " | EndOfScript@1:25",
            Tokenize("1 < 2 <= 3 != 4 == 5 % 6"));
    }

    [Fact]
    public void Tokenize_ReadsKeywordsIgnoringCaseAsLowerCase()
    {
        Assert.Equal(
            "Keyword if@1:1 | Identifier x@1:4 | Operator >=@1:6 | Number 10@1:9 | Keyword and@1:12"
            + " | Keyword not@1:16 | Identifier done@1:20 | NewLine@1:24 | Keyword end@2:1 | NewLine@2:4"
            + " | EndOfScript@2:4",
            Tokenize("If x >= 10 AND not done\nend"));
    }

    [Theory]
    [InlineData("clipboard.gettext", "Identifier clipboard.gettext@1:1")]
    [InlineData("a.b_c.d2", "Identifier a.b_c.d2@1:1")]
    [InlineData("if.x", "Identifier if.x@1:1")]
    [InlineData("_x", "Identifier _x@1:1")]
    public void Tokenize_ReadsDottedNamesAsOneIdentifier(string source, string expected)
    {
        int end = source.Length + 1;

        Assert.Equal(expected + $" | NewLine@1:{end} | EndOfScript@1:{end}", Tokenize(source));
    }

    [Fact]
    public void Tokenize_NewLineInsideParenthesesOrBracketsDoesNotEndTheStatement()
    {
        Assert.Equal(
            "Identifier say@1:1 | LeftParenthesis (@1:4 | Text a@1:5 | Comma ,@1:8 | LeftBracket [@2:3"
            + " | Number 1@2:4 | Comma ,@2:5 | Number 2@2:7 | RightBracket ]@2:8 | RightParenthesis )@2:9"
            + " | NewLine@2:10 | Identifier x@3:1 | NewLine@3:2 | EndOfScript@3:2",
            Tokenize("say(\"a\",\n  [1, 2])\nx"));
    }

    [Fact]
    public void Tokenize_ReadsTextEscapesAndSkipsTheComment()
    {
        Assert.Equal(
            "Identifier x@1:1 | Operator =@1:3 | Text a\"b\\c\\nd\\te@1:5 | NewLine@1:34 | EndOfScript@1:34",
            Tokenize("x = \"a\\\"b\\\\c\\nd\\te\" # comment \"x\""));
    }

    [Fact]
    public void Tokenize_CommentAndBlankLinesAddNoNewLineAndCrlfWorks()
    {
        Assert.Equal(
            "Identifier x@4:1 | Operator =@4:3 | Number 1@4:5 | NewLine@4:7 | Identifier y@5:1 | Operator =@5:3"
            + " | Number 2@5:5 | NewLine@5:7 | EndOfScript@6:1",
            Tokenize("# only\n\n  # comment\nx = 1\r\ny = 2\r\n"));
    }

    [Fact]
    public void Tokenize_MultiLineTextKeepsItsLinesWithoutTheClosingIndent()
    {
        Assert.Equal(
            "Identifier x@1:1 | Operator =@1:3 | Text a # b\\n  c\\n@1:5 | NewLine@5:8 | Identifier y@6:1"
            + " | Operator =@6:3 | Number 1@6:5 | NewLine@6:6 | EndOfScript@6:6",
            Tokenize("x = \"\"\"\n    a # b\n      c\n\n    \"\"\"\ny = 1"));
    }

    [Fact]
    public void Tokenize_ClosingLineOfMultiLineTextCanGoOn()
    {
        Assert.Equal(
            "Identifier f@1:1 | LeftParenthesis (@1:2 | Text t@1:3 | RightParenthesis )@3:6 | NewLine@3:7"
            + " | EndOfScript@3:7",
            Tokenize("f(\"\"\"\n  t\n  \"\"\")"));
    }

    [Theory]
    [InlineData("x = \"abc", 1, 5, "The text is not closed: add a \" at its end")]
    [InlineData("x = \"a\nb\"", 1, 5, "The text is not closed: add a \" at its end")]
    [InlineData("x = 1 !", 1, 7, "Unexpected character '!': use 'not' instead of '!'")]
    [InlineData("x = 'a'", 1, 5, "Unexpected character ''': texts use double quotes, \"like this\"")]
    [InlineData("x = @", 1, 5, "Unexpected character '@'")]
    [InlineData("x = 1.x", 1, 6, "Unexpected character '.'")]
    [InlineData("x = \"\\q\"", 1, 6, "Unknown escape \\q: use \\\" \\\\ \\n or \\t")]
    [InlineData("x = 99999999999999999999999999999", 1, 5, "The number 99999999999999999999999999999 is too large")]
    [InlineData("x = \"\"\"a\n\"\"\"", 1, 8, "Start the text on the line after \"\"\"")]
    [InlineData("x = \"\"\"\n a", 1, 5, "The text is not closed: add a line starting with \"\"\" after it")]
    [InlineData(
        "x = \"\"\"\n  a\n b\n  \"\"\"",
        3,
        1,
        "This line starts left of the closing \"\"\": indent it at least as much")]
    public void Tokenize_ReportsErrorsWithPosition(string source, int line, int column, string message)
    {
        // Act
        var error = Assert.Throws<ScriptException>(() => ScriptLexer.Tokenize(source));

        // Assert
        Assert.Equal((line, column, message), (error.Line, error.Column, error.Message));
    }

    [Fact]
    public void Tokenize_KeepsNothingFromAnEarlierCall()
    {
        // Arrange: an open ( would keep the next line in the same statement, were it remembered
        ScriptLexer.Tokenize("a = (1\nb = 2");

        // Act
        List<Token> tokens = ScriptLexer.Tokenize("c");

        // Assert
        Assert.Equal(
            [
                new Token(TokenType.Identifier, "c", 1, 1),
                new Token(TokenType.NewLine, "", 1, 2),
                new Token(TokenType.EndOfScript, "", 1, 2),
            ],
            tokens);
    }
}
