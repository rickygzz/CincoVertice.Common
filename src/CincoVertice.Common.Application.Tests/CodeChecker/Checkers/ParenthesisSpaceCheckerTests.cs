using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class ParenthesisSpaceCheckerTests
{
    private readonly ParenthesisSpaceChecker _checker = new();

    [Theory]
    [InlineData("internal void Read( LittleEndianByteArrayInputStream lei )")]
    [InlineData("Foo( a);")]
    [InlineData("Foo(a );")]
    [InlineData("Run( );")]
    public void Check_WhenParenthesisHasSpaceInside_AddsCh0035(string content)
    {
        var line = TestLine.Create("    " + content);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0035), error.Code);
        Assert.Equal(Errors.CH0035, error.Message);
    }

    [Theory]
    [InlineData("Foo(a, (b + c));")]
    [InlineData("Foo(")]
    [InlineData(");")]
    [InlineData("Foo( /* a */ b);")]
    [InlineData("var s = \"( a )\";")]
    [InlineData(" * ( a )")]
    public void Check_WhenNoSpaceInsideOrAtLineEdges_ReturnsNoErrors(string content)
    {
        var line = TestLine.Create("    " + content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData("    Foo(a); // ( b )")]
    [InlineData("    Foo(   // why")]
    public void CheckLine_WhenSpaceIsInOrBeforeComment_ReturnsNoCh0035(string content)
    {
        LineModel line = new() { Content = content, Number = 1 };

        new LineCheckerService().CheckLine(line);

        Assert.DoesNotContain(line.Errors, e => e.Code == nameof(Errors.CH0035));
    }
}
