using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class KeywordParenthesisSpaceCheckerTests
{
    private readonly KeywordParenthesisSpaceChecker _checker = new();

    [Theory]
    [InlineData("if(x)", "if")]
    [InlineData("while(x)", "while")]
    [InlineData("do(x)", "do")]
    [InlineData("for(int i = 0; i < 9; i++)", "for")]
    [InlineData("foreach(var item in items)", "foreach")]
    public void Check_WhenKeywordHasNoSpaceBeforeParenthesis_AddsCh0022(string content, string keyword)
    {
        var line = TestLine.Create("    " + content, number: 7);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0022), error.Code);
        Assert.Equal(Errors.CH0022.Replace("{keyword}", keyword), error.Message);
        Assert.Equal(7, error.Line);
    }

    [Theory]
    [InlineData("if  (x)", "if")]
    [InlineData("while   (x)", "while")]
    [InlineData("for  (int i = 0; i < 9; i++)", "for")]
    [InlineData("foreach  (var item in items)", "foreach")]
    public void Check_WhenKeywordHasMoreThanOneSpaceBeforeParenthesis_AddsCh0024(string content, string keyword)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0024), error.Code);
        Assert.Equal(Errors.CH0024.Replace("{keyword}", keyword), error.Message);
    }

    [Theory]
    [InlineData("var x = new (1, 2);")]
    [InlineData("Point p = new  (1, 2);")]
    public void Check_WhenNewHasSpaceBeforeParenthesis_AddsCh0023(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0023), error.Code);
        Assert.Equal(Errors.CH0023.Replace("{keyword}", "new"), error.Message);
    }

    [Fact]
    public void Check_WhenLineHasKeywordAndNewErrors_AddsBoth()
    {
        var line = TestLine.Create("if(x) y = new (1);");

        _checker.Check(line);

        Assert.Equal([nameof(Errors.CH0023), nameof(Errors.CH0022)], line.Errors.Select(e => e.Code));
    }

    [Theory]
    [InlineData("if (x)")]
    [InlineData("while (x)")]
    [InlineData("do")]
    [InlineData("do {")]
    [InlineData("for (int i = 0; i < 9; i++)")]
    [InlineData("foreach (var item in items)")]
    [InlineData("var x = new();")]
    [InlineData("var x = new Point(1, 2);")]
    // Identifiers that start with a keyword
    [InlineData("double x = 1;")]
    [InlineData("iffy(x);")]
    [InlineData("format(x);")]
    [InlineData("forward(x);")]
    [InlineData("whileLoop(x);")]
    public void Check_WhenSpacingIsCorrect_AddsNoError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }
}
