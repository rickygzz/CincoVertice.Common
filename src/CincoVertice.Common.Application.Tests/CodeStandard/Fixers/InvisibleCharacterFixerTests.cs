using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class InvisibleCharacterFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new InvisibleCharacterFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("var\u00A0x = 1;")]   // no-break space
    [InlineData("var\u2002x = 1;")]   // en space
    [InlineData("var\u2003x = 1;")]   // em space
    [InlineData("var\u2009x = 1;")]   // thin space
    [InlineData("var\u202Fx = 1;")]   // narrow no-break space
    [InlineData("var\u205Fx = 1;")]   // medium mathematical space
    [InlineData("var\u3000x = 1;")]   // ideographic space
    public void Fix_ReplacesUnicodeSpacesWithNormalSpace(string content)
    {
        // Act
        var result = new InvisibleCharacterFixer().Fix(content);

        // Assert
        Assert.Equal("var x = 1;", result);
    }

    [Theory]
    [InlineData("va\u200Br x = 1;")]   // zero width space
    [InlineData("va\u200Cr x = 1;")]   // zero width non-joiner
    [InlineData("va\u200Dr x = 1;")]   // zero width joiner
    [InlineData("va\u2060r x = 1;")]   // word joiner
    [InlineData("va\u00ADr x = 1;")]   // soft hyphen
    [InlineData("va\uFEFFr x = 1;")]   // byte order mark in the middle
    [InlineData("va\u200Er x = 1;")]   // left-to-right mark
    [InlineData("va\u202Er x = 1;")]   // right-to-left override (Trojan Source)
    [InlineData("va\u2066r x = 1;")]   // left-to-right isolate
    public void Fix_RemovesInvisibleCharacters(string content)
    {
        // Act
        var result = new InvisibleCharacterFixer().Fix(content);

        // Assert
        Assert.Equal("var x = 1;", result);
    }

    [Fact]
    public void Fix_KeepsByteOrderMarkAtStart()
    {
        // Act
        var result = new InvisibleCharacterFixer().Fix("\uFEFFvar x = 1;");

        // Assert
        Assert.Equal("\uFEFFvar x = 1;", result);
    }

    [Fact]
    public void Fix_FixesIndentationWithNoBreakSpaces()
    {
        // Act
        var result = new InvisibleCharacterFixer().Fix("{\n\u00A0\u00A0\u00A0\u00A0x = 1;\n}");

        // Assert
        Assert.Equal("{\n    x = 1;\n}", result);
    }

    [Theory]
    [InlineData("// Comentario: añadir la opción ¿sí?")]
    [InlineData("string s = \"Ñandú — “ok” …\";")]
    [InlineData("var emoji = \"😀\";")]
    [InlineData("var x = 1;\r\n\tvar y = 2;\r\n")]
    public void Fix_KeepsVisibleCharacters(string content)
    {
        // Act
        var result = new InvisibleCharacterFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }
}
