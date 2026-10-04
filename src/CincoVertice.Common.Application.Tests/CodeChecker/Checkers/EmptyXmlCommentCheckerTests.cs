using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class EmptyXmlCommentCheckerTests
{
    private readonly EmptyXmlCommentChecker _checker = new();

    [Theory]
    [InlineData("/// <summary>")]
    [InlineData("    ///     Checks the line.")]
    [InlineData("/// <param name=\"line\">The line.</param>")]
    [InlineData("/// <returns>True if valid.</returns>")]
    [InlineData("/// <inheritdoc/>")]
    [InlineData("/// <summary></remarks>")]
    [InlineData("///")]
    public void Check_WhenDocumentationHasText_ContinuesWithoutError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData("/// <summary></summary>")]
    [InlineData("/// <returns></returns>")]
    [InlineData("/// <param name=\"line\"></param>")]
    [InlineData("/// <remarks></remarks>")]
    [InlineData("/// <typeparam name=\"T\"></typeparam>")]
    [InlineData("    /// <returns></returns> The value.")]
    public void Check_WhenTagIsEmpty_AddsCh0030AndStops(string content)
    {
        var line = TestLine.Create(content, number: 10);

        var result = _checker.Check(line);

        Assert.False(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0030), error.Code);
        Assert.Equal(Errors.CH0030, error.Message);
        Assert.Equal(10, error.Line);
    }

    [Theory]
    [InlineData("var x = 1;")]
    [InlineData("// <summary></summary>")]
    [InlineData("//// <summary></summary>")]
    [InlineData("///<summary>")]
    public void Check_WhenLineIsNotDocumentationOrHasNoEmptyTag_ContinuesWithoutError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }
}
