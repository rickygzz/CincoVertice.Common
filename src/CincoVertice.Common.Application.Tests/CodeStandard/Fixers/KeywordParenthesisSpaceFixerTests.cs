using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class KeywordParenthesisSpaceFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("if(a == b)", "if (a == b)")]
    [InlineData("if  (a == b)", "if (a == b)")]
    [InlineData("for(int i = 0; i < n; i++)", "for (int i = 0; i < n; i++)")]
    [InlineData("for   (int i = 0; i < n; i++)", "for (int i = 0; i < n; i++)")]
    [InlineData("foreach(var a in list)", "foreach (var a in list)")]
    [InlineData("foreach    (var a in list)", "foreach (var a in list)")]
    [InlineData("while(a > b)", "while (a > b)")]
    [InlineData("while     (a > b)", "while (a > b)")]
    public void Fix_WhenNoSpaceBeforeParenthesis_AddsSingleSpace(string content, string expected)
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("if  (a == b)", "if (a == b)")]
    [InlineData("for   (int i = 0;)", "for (int i = 0;)")]
    [InlineData("foreach  (var a in list)", "foreach (var a in list)")]
    [InlineData("while    (a > b)", "while (a > b)")]
    public void Fix_WhenMultipleSpacesBeforeParenthesis_ReducesToSingleSpace(string content, string expected)
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("} while     (a > b)", "} while (a > b)")]
    [InlineData("}    while     (a > b)", "} while (a > b)")]
    [InlineData("    }    while     (a > b)", "    } while (a > b)")]
    [InlineData("    }    if     (a > b)", "    }\n    if (a > b)")]
    [InlineData("        }    if     (a > b)", "        }\n        if (a > b)")]
    [InlineData("    }    else if     (a > b)", "    }\n    else if (a > b)")]
    [InlineData("        }    else if     (a > b)", "        }\n        else if (a > b)")]
    public void Fix_WhenLineStartsWithBraces_IndentProperly(string content, string expected)
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("while     (a > b)   {", "while (a > b)\n{")]
    [InlineData("    while     (a > b)   {", "    while (a > b)\n    {")]
    [InlineData("    do     (a > b)    {", "    do (a > b)\n    {")]
    [InlineData("        if     (a > b)  {", "        if (a > b)\n        {")]
    [InlineData("    }    else if     (a > b)    {", "    }\n    else if (a > b)\n    {")]
    public void Fix_WhenLineEndsWithOpenBrace_IndentProperly(string content, string expected)
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Fix_WhenTabBeforeParenthesis_NormalizesToSingleSpace()
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix("if\t(a == b)");

        // Assert
        Assert.Equal("if (a == b)", result);
    }

    [Theory]
    [InlineData("if (a == b)")]
    [InlineData("foreach (var a in list)")]
    [InlineData("    while (a > b)")]
    public void Fix_WhenAlreadyOneSpace_LeavesUnchanged(string content)
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Theory]
    [InlineData("verify(x)")]        // ends in "ify", not the keyword "if"
    [InlineData("notify(x)")]
    [InlineData("DoWork(x)")]        // PascalCase method, not "do"
    [InlineData("form(x)")]          // contains "for"
    [InlineData("todo(x)")]          // contains "do"
    public void Fix_DoesNotAffectIdentifiersContainingKeywords(string content)
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_DoesNotAffectNewKeyword()
    {
        // Act - "new" is out of scope (it should have no space, handled elsewhere)
        var result = new KeywordParenthesisSpaceFixer().Fix("new  (x)");

        // Assert
        Assert.Equal("new  (x)", result);
    }

    [Fact]
    public void Fix_FixesMultipleOccurrencesOnSameLine()
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix("if(x) { } else if  (y) { }");

        // Assert
        Assert.Equal("if (x) { } else if (y) { }", result);
    }

    [Theory]
    [InlineData("if(x)\r\nwhile  (y)", "if (x)\r\nwhile (y)")]
    [InlineData("if(x)\nwhile  (y)", "if (x)\nwhile (y)")]
    public void Fix_FixesEachLineAndPreservesLineEndings(string content, string expected)
    {
        // Act
        var result = new KeywordParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }
}
