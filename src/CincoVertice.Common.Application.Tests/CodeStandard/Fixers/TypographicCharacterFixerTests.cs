using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class TypographicCharacterFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new TypographicCharacterFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("// It’s “quoted” — and – more…", "// It's \"quoted\" - and - more...")]
    [InlineData("/// <summary>Returns ‘x’ − 1.</summary>", "/// <summary>Returns 'x' - 1.</summary>")]
    [InlineData("/* Block — comment… */", "/* Block - comment... */")]
    [InlineData("/* Line one —\n   line two … */", "/* Line one -\n   line two ... */")]
    [InlineData("var x = 1; // Trailing — comment", "var x = 1; // Trailing - comment")]
    [InlineData("// Unclosed block /* — still a line comment", "// Unclosed block /* - still a line comment")]
    [InlineData("/* Unclosed — block", "/* Unclosed - block")]
    public void Fix_ReplacesTypographicCharactersInComments(string content, string expected)
    {
        // Act
        var result = new TypographicCharacterFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("var x = a − b;", "var x = a - b;")]
    [InlineData("var s = “hello”;", "var s = \"hello\";")]
    [InlineData("var s = “it’s”;", "var s = \"it's\";")]
    [InlineData("var c = ‘x’;", "var c = 'x';")]
    [InlineData("if (a – b > 0) { }", "if (a - b > 0) { }")]
    [InlineData("var s = \"—\"; var x = a − b;", "var s = \"—\"; var x = a - b;")]
    [InlineData("var x = a − b; // c — d", "var x = a - b; // c - d")]
    [InlineData("var @class = a − b;", "var @class = a - b;")]
    [InlineData("var s = $\"{a}—\" + b − c;", "var s = $\"{a}—\" + b - c;")]
    public void Fix_ReplacesTypographicCharactersInCode(string content, string expected)
    {
        // Act
        var result = new TypographicCharacterFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        var fixer = new TypographicCharacterFixer();
        string once = fixer.Fix("var s = “a — b”; // it’s…\nvar x = 1 − 2;");

        // Act
        string twice = fixer.Fix(once);

        // Assert
        Assert.Equal("var s = \"a - b\"; // it's...\nvar x = 1 - 2;", once);
        Assert.Equal(once, twice);
    }

    [Theory]
    [InlineData("var s = \"It’s — “ok” …\";")]
    [InlineData("var s = @\"It’s — \"\"ok\"\" …\";")]
    [InlineData("var s = $\"{name} — “ok”\";")]
    [InlineData("var s = $@\"{name} — “ok”\";")]
    [InlineData("var s = @$\"{name} — “ok”\";")]
    [InlineData("var s = \"\"\"\n    Raw — “text” with \"quotes\"\n    \"\"\";")]
    [InlineData("var c = '—';")]
    [InlineData("var s = \"// not a comment — \";")]
    [InlineData("var s = \"/* not a comment — */\";")]
    [InlineData("var s = \"escaped \\\" — still string\";")]
    [InlineData("var c = '\\'';\nvar s = \"—\";")]
    public void Fix_DoesNotChangeStringOrCharLiterals(string content)
    {
        // Act
        var result = new TypographicCharacterFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Theory]
    [InlineData("var s = \"—\"; // —", "var s = \"—\"; // -")]
    [InlineData("var s = @\"a\n—\"; // —", "var s = @\"a\n—\"; // -")]
    [InlineData("var c = '\"'; // —", "var c = '\"'; // -")]
    [InlineData("var @class = 1; // —", "var @class = 1; // -")]
    [InlineData("var s = \"\"; // —", "var s = \"\"; // -")]
    public void Fix_ReplacesInCommentAfterLiteral(string content, string expected)
    {
        // Act
        var result = new TypographicCharacterFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("// Comentario: añadir la opción «sí» ¿y? ¡no!")]
    [InlineData("var x = a - b; // plain ASCII - unchanged...")]
    public void Fix_KeepsOtherCharacters(string content)
    {
        // Act
        var result = new TypographicCharacterFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }
}
