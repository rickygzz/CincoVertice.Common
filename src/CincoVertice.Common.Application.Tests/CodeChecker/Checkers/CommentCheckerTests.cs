using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class CommentCheckerTests
{
    private readonly CommentChecker _checker = new();

    [Theory]
    [InlineData("// Comment")]
    [InlineData("    // Indented comment")]
    [InlineData("//")]
    [InlineData("/// <summary>")]
    public void Check_WhenLineIsValidComment_StopsWithoutError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.False(result);
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData("//Comment")]
    [InlineData("    //TODO")]
    [InlineData("//-")]
    public void Check_WhenNoSpaceAfterSlashes_AddsErrorAndStops(string content)
    {
        var line = TestLine.Create(content, number: 8);

        var result = _checker.Check(line);

        Assert.False(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0021), error.Code);
        Assert.Equal(Errors.CH0021, error.Message);
        Assert.Equal(8, error.Line);
    }

    [Theory]
    [InlineData("var x = 1;")]
    [InlineData("var url = \"http://example.com\";")]
    [InlineData("var s = @\"// not a comment\";")]
    [InlineData("var c = '/';")]
    [InlineData("x = a /* // inside block comment */ + b;")]
    [InlineData("/* Block comment */")]
    public void Check_WhenLineHasNoComment_ContinuesWithTrimmedContentUnchanged(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
        Assert.Equal(content, line.TrimmedContent);
    }

    [Theory]
    [InlineData("if (a != b) // some comment", "if (a != b)")]
    [InlineData("    var x = 1;   // Indented code", "var x = 1;")]
    [InlineData("var x = 1; //", "var x = 1;")]
    [InlineData("var x = 1; /// Triple slash", "var x = 1;")]
    [InlineData("var url = \"http://example.com\"; // Link", "var url = \"http://example.com\";")]
    [InlineData("var c = '/'; // Slash", "var c = '/';")]
    [InlineData("x = a /* block */ + b; // Line", "x = a /* block */ + b;")]
    public void Check_WhenLineHasTrailingComment_RemovesItFromTrimmedContent(string content, string expectedCode)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
        Assert.Equal(expectedCode, line.TrimmedContent);
    }

    [Theory]
    [InlineData("if (a != b) //some comment", "if (a != b)")]
    [InlineData("var x = 1; //TODO", "var x = 1;")]
    public void Check_WhenTrailingCommentHasNoSpaceAfterSlashes_AddsErrorAndContinues(
        string content,
        string expectedCode)
    {
        var line = TestLine.Create(content, number: 11);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0021), error.Code);
        Assert.Equal(11, error.Line);
        Assert.Equal(expectedCode, line.TrimmedContent);
    }
}
