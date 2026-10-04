using CincoVertice.Common.Application.CodeStandard.Helper;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Helper;

public class LiteralMaskTests
{
    [Theory]
    [InlineData("var s = \"a\";")]
    [InlineData("var s = @\"line   \n  indented\n\";\nx();")]
    [InlineData("var s = \"\"\"\n    raw { }\n    \"\"\";")]
    [InlineData("var s = $\"{a} if(x)\";")]
    [InlineData("var c = '{'; var d = '\"';")]
    [InlineData("var @class = 1; // it's \"quoted\"")]
    [InlineData("")]
    public void MaskThenRestore_ReturnsOriginal(string content)
    {
        // Arrange
        var mask = new LiteralMask();

        // Act
        string restored = mask.Restore(mask.Mask(content));

        // Assert
        Assert.Equal(content, restored);
    }

    [Fact]
    public void Mask_PutsMultiLineLiteralOnOneLine()
    {
        // Act
        string masked = new LiteralMask().Mask("var s = @\"a\n  b\n\";\nx();");

        // Assert
        Assert.Equal(1, masked.Count(c => c == '\n'));
        Assert.EndsWith(";\nx();", masked);
    }

    [Fact]
    public void Mask_HidesBracesAndParenthesesInLiterals()
    {
        // Act
        string masked = new LiteralMask().Mask("var c = '{'; var s = \"} if(x)\";");

        // Assert
        Assert.DoesNotContain('{', masked);
        Assert.DoesNotContain('}', masked);
        Assert.DoesNotContain("if(", masked);
    }

    [Fact]
    public void Mask_LeavesCommentsUnchanged()
    {
        // Arrange
        string content = "x(); // it's \"quoted\"\n/* \"block\" */";

        // Act
        string masked = new LiteralMask().Mask(content);

        // Assert
        Assert.Equal(content, masked);
    }

    [Fact]
    public void Restore_WhenPlaceholderLineMoved_RestoresItWhereItIs()
    {
        // Arrange
        var mask = new LiteralMask();
        string masked = mask.Mask("a = \"x\";\nb = \"y\";");
        string[] lines = masked.Split('\n');

        // Act
        string restored = mask.Restore(lines[1] + "\n" + lines[0]);

        // Assert
        Assert.Equal("b = \"y\";\na = \"x\";", restored);
    }
}
