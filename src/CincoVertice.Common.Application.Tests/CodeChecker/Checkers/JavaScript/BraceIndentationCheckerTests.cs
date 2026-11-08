using CincoVertice.Common.Application.CodeStandard.Checkers.JavaScript;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers.JavaScript;

public class BraceIndentationCheckerTests
{
    private readonly BraceIndentationChecker _checker = new();

    [Theory]
    [InlineData("  a();", 4)]
    [InlineData("\ta();", 4)]
    [InlineData("  \t  a();", 4)]
    [InlineData("a();", 8)]
    [InlineData("        a();", 4)]
    public void Check_WhenTheIndentationIsntTheExpectedOne_AddsCh0036(string content, int expected)
    {
        // Arrange
        var line = TestLine.Create(content);
        line.ExpectedIndentation = expected;

        // Act
        bool result = _checker.Check(line);

        // Assert
        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0036), error.Code);
        Assert.Equal($"Line should be indented {expected} spaces, by the brackets around it.", error.Message);
    }

    [Theory]
    [InlineData("    a();", 4)]
    [InlineData("a();", 0)]
    [InlineData("  text of a template", null)]
    [InlineData("", 4)]
    [InlineData("   ", 8)]
    public void Check_WhenItIsRightOrLeftAlone_ReturnsNoErrors(string content, int? expected)
    {
        // Arrange
        var line = TestLine.Create(content);
        line.ExpectedIndentation = expected;

        // Act
        bool result = _checker.Check(line);

        // Assert
        Assert.True(result);
        Assert.Empty(line.Errors);
    }
}
