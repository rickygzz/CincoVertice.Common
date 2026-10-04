using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class CommentSpaceFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new CommentSpaceFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("//comment", "// comment")]
    [InlineData("    //TODO: fix", "    // TODO: fix")]
    [InlineData("///<summary>", "/// <summary>")]
    [InlineData("    ///Text.", "    /// Text.")]
    [InlineData("//\tTabbed", "// Tabbed")]
    [InlineData("if (a != b) //some comment", "if (a != b) // some comment")]
    [InlineData("var x = 1;//comment", "var x = 1;// comment")]
    [InlineData("//var x = 1;", "// var x = 1;")]
    public void Fix_AddsSpaceAfterSlashes(string content, string expected)
    {
        // Act
        var result = new CommentSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("// Comment")]
    [InlineData("/// <summary>")]
    [InlineData("//")]
    [InlineData("///")]
    [InlineData("var x = 1; //")]
    [InlineData("////////////")]
    [InlineData("////Commented out doc")]
    [InlineData("/*comment*/")]
    [InlineData("/*\n//not a line comment\n*/")]
    [InlineData("var url = \"http://example.com\";")]
    [InlineData("var s = @\"//a\";")]
    [InlineData("var s = $\"//{x}\";")]
    [InlineData("var c = '/';")]
    public void Fix_WhenNothingToFix_ReturnsContentUnchanged(string content)
    {
        // Act
        var result = new CommentSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_FixesEveryCommentAndKeepsLineEndings()
    {
        // Arrange
        string content = "//first\r\nvar x = 1; //second\r\n\r\n///<summary>\n// ok\n//last";

        // Act
        var result = new CommentSpaceFixer().Fix(content);

        // Assert
        Assert.Equal("// first\r\nvar x = 1; // second\r\n\r\n/// <summary>\n// ok\n// last", result);
    }

    [Fact]
    public void Fix_IgnoresSlashesInMultiLineVerbatimString()
    {
        // Arrange
        string content = "var s = @\"line one\n//not a comment\";\nvar y = 2; //comment";

        // Act
        var result = new CommentSpaceFixer().Fix(content);

        // Assert
        Assert.Equal("var s = @\"line one\n//not a comment\";\nvar y = 2; // comment", result);
    }

    [Fact]
    public void Fix_IgnoresSlashesInRawString()
    {
        // Arrange
        string content = "var s = \"\"\"\n    //not a comment\n    \"\"\";\n//comment";

        // Act
        var result = new CommentSpaceFixer().Fix(content);

        // Assert
        Assert.Equal("var s = \"\"\"\n    //not a comment\n    \"\"\";\n// comment", result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        var fixer = new CommentSpaceFixer();
        string once = fixer.Fix("//a\n///b\nx; //c");

        // Act
        string twice = fixer.Fix(once);

        // Assert
        Assert.Equal(once, twice);
    }
}
