using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class IndentationFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new IndentationFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Fix_WhenLineStartsWithTab_ConvertsToFourSpaces()
    {
        // Act
        var result = new IndentationFixer().Fix("\tvar a = 1;");

        // Assert
        Assert.Equal("    var a = 1;", result);
    }

    [Fact]
    public void Fix_WhenLineStartsWithMultipleTabs_ConvertsEachToFourSpaces()
    {
        // Act
        var result = new IndentationFixer().Fix("\t\tvar a = 1;");

        // Assert
        Assert.Equal("        var a = 1;", result);
    }

    [Theory]
    [InlineData("    var a = 1;")]      // 4 spaces
    [InlineData("        var a = 1;")]  // 8 spaces
    public void Fix_WhenIndentationIsAlreadyMultipleOfFour_LeavesItUnchanged(string content)
    {
        // Act
        var result = new IndentationFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Theory]
    [InlineData(" var a = 1;", "var a = 1;")]              // 1 -> 0
    [InlineData("  var a = 1;", "    var a = 1;")]         // 2 -> 4
    [InlineData("   var a = 1;", "    var a = 1;")]        // 3 -> 4
    [InlineData("     var a = 1;", "    var a = 1;")]      // 5 -> 4
    [InlineData("      var a = 1;", "        var a = 1;")] // 6 -> 8
    public void Fix_RoundsSpaceIndentationToNearestMultipleOfFour(string content, string expected)
    {
        // Act
        var result = new IndentationFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Fix_WhenTabsAndSpacesAreMixed_NormalizesToSpaces()
    {
        // Act - one tab (4) + four spaces (4) = 8 columns -> 8 spaces
        var result = new IndentationFixer().Fix("\t    var a = 1;");

        // Assert
        Assert.Equal("        var a = 1;", result);
    }

    [Fact]
    public void Fix_OnlyConvertsLeadingTabs_NotTabsWithinTheLine()
    {
        // Act
        var result = new IndentationFixer().Fix("\tvar a =\tb;");

        // Assert
        Assert.Equal("    var a =\tb;", result);
    }

    [Fact]
    public void Fix_LeavesWhitespaceOnlyLineUnchanged()
    {
        // Act
        var result = new IndentationFixer().Fix("\t\t");

        // Assert
        Assert.Equal("\t\t", result);
    }

    [Theory]
    [InlineData("\tvar a = 1;\r\nvar b = 2;", "    var a = 1;\r\nvar b = 2;")]
    [InlineData("\tvar a = 1;\nvar b = 2;", "    var a = 1;\nvar b = 2;")]
    public void Fix_PreservesLineEndings(string content, string expected)
    {
        // Act
        var result = new IndentationFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Fix_FixesEveryLineIndependently()
    {
        // Arrange
        string content = "\tclass Test\n\t{\n\t\tint value;\n\t}";

        // Act
        var result = new IndentationFixer().Fix(content);

        // Assert
        Assert.Equal("    class Test\n    {\n        int value;\n    }", result);
    }

    [Fact]
    public void Fix_WhenContentEndsWithNewline_DoesNotAppendExtraLine()
    {
        // Act
        var result = new IndentationFixer().Fix("\tvar a = 1;\n");

        // Assert
        Assert.Equal("    var a = 1;\n", result);
    }
}
