using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class InvisibleCharacterCheckerTests
{
    private readonly InvisibleCharacterChecker _checker = new();

    [Theory]
    [InlineData("var x = 1;")]
    [InlineData("// Comentario: añadir la opción ¿sí? «ok» — “x” …")]
    [InlineData("\tvar emoji = \"😀\";")]
    public void Check_WhenLineHasOnlyVisibleCharacters_AddsNoErrors(string content)
    {
        var line = new LineModel { Content = content, Number = 1 };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void Check_WhenLineHasInvisibleCharacter_ReportsCodePointAndColumn()
    {
        var line = new LineModel { Content = "va\u200Br x = 1;", Number = 7 };

        var result = _checker.Check(line);

        Assert.True(result);
        ErrorModel error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0025), error.Code);
        Assert.Equal(7, error.Line);
        Assert.Equal("Line contains invisible character U+200B (zero width space) at column 3.", error.Message);
    }

    [Fact]
    public void Check_WhenLineHasUnicodeSpace_ReportsCodePointAndColumn()
    {
        var line = new LineModel { Content = "var\u00A0x = 1;", Number = 1 };

        _checker.Check(line);

        ErrorModel error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0026), error.Code);
        Assert.Equal(
            "Line contains Unicode space U+00A0 (no-break space) at column 4; use a normal space.",
            error.Message);
    }

    [Fact]
    public void Check_WhenCharacterHasNoName_ReportsCodePointOnly()
    {
        var line = new LineModel { Content = "a\u202Eb", Number = 1 };

        _checker.Check(line);

        Assert.Equal("Line contains invisible character U+202E at column 2.", Assert.Single(line.Errors).Message);
    }

    [Fact]
    public void Check_ReportsEveryOccurrence()
    {
        var line = new LineModel { Content = "\u00A0a\u200Bb\u00A0", Number = 1 };

        _checker.Check(line);

        Assert.Equal(
            [nameof(Errors.CH0026), nameof(Errors.CH0025), nameof(Errors.CH0026)],
            line.Errors.Select(e => e.Code));
    }

    [Theory]
    [InlineData("    // Comment with zero\u200Bwidth space")]
    [InlineData("    /// <summary>Doc\u00A0comment</summary>")]
    [InlineData("\u00A0\u00A0\u00A0\u00A0")]
    public void CheckLine_ReportsInvisibleCharactersOnCommentAndWhitespaceLines(string content)
    {
        var line = new LineModel { Content = content, Number = 1 };

        new LineCheckerService().CheckLine(line);

        Assert.Contains(line.Errors, e => e.Code is nameof(Errors.CH0025) or nameof(Errors.CH0026));
    }
}
