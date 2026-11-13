using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class MultipleSpaceCheckerTests
{
    private readonly MultipleSpaceChecker _checker = new();

    [Theory]
    [InlineData("private byte[] field_1_UID  = new byte[16];")]
    [InlineData("var x =   1;")]
    [InlineData("return  Foo(a, b);")]
    [InlineData("var s = \"a  b\" +  c;")]
    public void Check_WhenCodeHasConsecutiveSpaces_AddsCh0032(string content)
    {
        var line = TestLine.Create("    " + content);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0032), error.Code);
        Assert.Equal(Errors.CH0032, error.Message);
    }

    [Theory]
    [InlineData("        var x = 1;")]
    [InlineData("var s = \"a  b\";")]
    [InlineData("var s = $\"{a}  {b}\";")]
    [InlineData("var c = ' ';")]
    [InlineData("var x = 1;  /* note */")]
    [InlineData("var x = /* a  b */ 1;")]
    [InlineData(" *  table:  a  b")]
    [InlineData("/*  a  b")]
    [InlineData("if  (a == b)")]
    [InlineData("var a = new  ();")]
    public void Check_WhenSpacesAreIndentationLiteralsOrComments_ReturnsNoErrors(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData("    var x = 1;  // note")]
    [InlineData("    //  comment  with  spaces")]
    public void CheckLine_WhenSpacesAreBeforeOrInsideLineComment_ReturnsNoCh0032(string content)
    {
        LineModel line = new() { Content = content, Number = 1 };

        new LineCheckerService().CheckLine(line);

        Assert.DoesNotContain(line.Errors, e => e.Code == nameof(Errors.CH0032));
    }
}
