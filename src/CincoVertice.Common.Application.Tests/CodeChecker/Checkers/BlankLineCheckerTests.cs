using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class BlankLineCheckerTests
{
    private readonly BlankLineChecker _checker = new();

    [Theory]
    [InlineData("var x = 1;", false)]
    [InlineData("var x = 1;", true)]
    [InlineData("}", false)]
    [InlineData("    { }", true)]
    public void Check_WhenLineHasCode_ContinuesWithoutError(string content, bool previousLineIsBlank)
    {
        var line = TestLine.Create(content, previousLineIsBlank: previousLineIsBlank);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void Check_WhenSingleEmptyLine_StopsWithoutError()
    {
        var line = TestLine.Create(string.Empty);

        var result = _checker.Check(line);

        Assert.False(result);
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void Check_WhenEmptyLineFollowsBlankLine_AddsCh0027AndStops()
    {
        var line = TestLine.Create(string.Empty, number: 7, previousLineIsBlank: true);

        var result = _checker.Check(line);

        Assert.False(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0027), error.Code);
        Assert.Equal(Errors.CH0027, error.Message);
        Assert.Equal(7, error.Line);
    }

    [Fact]
    public void Check_WhenWhitespaceOnlyLineFollowsBlankLine_AddsCh0027AndContinues()
    {
        var line = TestLine.Create("    ", previousLineIsBlank: true);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(nameof(Errors.CH0027), Assert.Single(line.Errors).Code);
    }

    [Theory]
    [InlineData("}")]
    [InlineData("    }")]
    [InlineData("    });")]
    public void Check_WhenClosingBraceFollowsBlankLine_AddsCh0028AndContinues(string content)
    {
        var line = TestLine.Create(content, number: 4, previousLineIsBlank: true);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0028), error.Code);
        Assert.Equal(Errors.CH0028, error.Message);
        Assert.Equal(4, error.Line);
    }
}
