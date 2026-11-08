using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class MultipleSpaceFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new MultipleSpaceFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("    private byte[] field_1_UID  = new byte[16];\n", "    private byte[] field_1_UID = new byte[16];\n")]
    [InlineData("var x =   1;\n", "var x = 1;\n")]
    [InlineData("return  Foo(a,  b);\n", "return Foo(a, b);\n")]
    [InlineData("a  =  b;\r\nc  =  d;\r\n", "a = b;\r\nc = d;\r\n")]
    public void Fix_ReplacesConsecutiveSpacesInCodeWithOne(string content, string expected)
    {
        // Act
        var result = new MultipleSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("        var x = 1;\n")]
    [InlineData("var x = 1;   \n")]
    [InlineData("var x = 1;  // note\n")]
    [InlineData("var x = 1;  /* note */\n")]
    [InlineData("//  comment  with  spaces\n")]
    [InlineData("var x = 1; // a  b\n")]
    [InlineData("/*\n *  table:  a  b\n */\n")]
    [InlineData("var s = \"a  b\";\n")]
    [InlineData("var s = @\"a  b\n    c  d\";\n")]
    [InlineData("var c = ' ';\n")]
    [InlineData("var s = $\"{d[\"k\"]}  {x}\";\n")]
    public void Fix_LeavesIndentationLineEndsCommentsAndLiteralsUnchanged(string content)
    {
        // Act
        var result = new MultipleSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        var fixer = new MultipleSpaceFixer();
        string once = fixer.Fix("int  a  =  \"x  y\".Length;  // c  d\n");

        // Act
        string twice = fixer.Fix(once);

        // Assert
        Assert.Equal("int a = \"x  y\".Length;  // c  d\n", once);
        Assert.Equal(once, twice);
    }
}
