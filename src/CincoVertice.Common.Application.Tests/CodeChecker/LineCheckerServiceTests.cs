using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker;

public class LineCheckerServiceTests
{
    private readonly LineCheckerService _lineChecker = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void CheckLine_EmptyLine_AddsError(string? lineContent)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent!,
            Number = 1
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData("    ")]
    [InlineData("\t\t")]
    public void CheckLine_LineWithWhitespaceOnly_AddsError(string lineContent)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent!,
            Number = 2
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0010), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0010, line.Errors[0].Message);
    }

    [Fact]
    public void CheckLine_LineEndsWithWhitespace_AddsError()
    {
        // Arrange
        LineModel line = new()
        {
            Content = "Some code here    ",
            Number = 3
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0011), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0011, line.Errors[0].Message);
    }

    [Fact]
    public void CheckLine_LineIsTabIndented_AddsError()
    {
        // Arrange
        LineModel line = new()
        {
            Content = "\tSome code here    ",
            Number = 4
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Equal(2, line.Errors.Count);
        Assert.Equal(nameof(Errors.CH0011), line.Errors[0].Code);
        Assert.Equal(nameof(Errors.CH0012), line.Errors[1].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0011, line.Errors[0].Message);
        Assert.Equal(Errors.CH0012, line.Errors[1].Message);
    }

    [Theory]
    [InlineData(" var a = 1;")]
    [InlineData("  var a = 1;")]
    [InlineData("   var a = 1;")]
    [InlineData("     var a = 1;")]
    public void CheckLine_LineIndentionIsMultipleOfFour_AddsError(string lineContent)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 4
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0013), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0013, line.Errors[0].Message);
    }

    [Fact]
    public void CheckLine_LineExceeds120Characters_AddsError()
    {
        // Arrange
        LineModel line = new()
        {
            Content = new('a', 121),
            Number = 5
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0016), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0016, line.Errors[0].Message);
    }

    [Theory]
    [InlineData("    ; NextStatement()", ";")]
    [InlineData("    , string parameter2,", ",")]
    [InlineData("    = 10", "=")]
    public void CheckLine_LineStartsWithForbiddenToken_AddsError(string lineContent, string token)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 6
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0017), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0017.Replace("{token}", token), line.Errors[0].Message);
    }

    [Theory]
    [InlineData("    () =>")]
    [InlineData("    (x) =>")]
    [InlineData("    (x , y) =>")]
    [InlineData("    (Min , Max) =")]
    [InlineData("    (x, y, z) =>")]
    public void CheckLine_LineStartsWithForbiddenToken_NoError(string lineContent)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 6
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData("    total > 100 &&", "&&")]
    [InlineData("    a > total && !", "!")]
    public void CheckLine_LineEndsWithForbiddenToken_AddsError(string lineContent, string token)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 7
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0018), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0018.Replace("{token}", token), line.Errors[0].Message);
    }

    [Theory]
    [InlineData("    case 'A':")]
    [InlineData("    default:")]
    [InlineData("    default :")]
    [InlineData("    var listA = listB!")]
    public void CheckLine_LineEndsWithForbiddenToken_NoError(string lineContent)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 7
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void CheckLine_LineContainsForbiddenToken_AddsError()
    {
        // Arrange
        LineModel line = new()
        {
            Content = "this. is forbidden",
            Number = 8
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0019), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0019.Replace("{token}", "this."), line.Errors[0].Message);
    }

    [Theory]
    [InlineData("    ///<summary>The summary.</summary>")]
    [InlineData("    ///<returns>Returns a list of products.</returns>")]
    public void CheckLine_ContainsNoSpaceInDocumentation_AddsError(string lineContent)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 9
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0020), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0020, line.Errors[0].Message);
    }

    [Theory]
    [InlineData("    //A comment")]
    [InlineData("    //TODO: Refactor this code.")]
    public void CheckLine_ContainsNoSpaceInComment_AddsError(string lineContent)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 9
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0021), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0021, line.Errors[0].Message);
    }

    [Theory]
    [InlineData("    if(a == b)", "if")]
    [InlineData("    for(int i=0; i<total; i++)", "for")]
    [InlineData("    foreach(var a in list)", "foreach")]
    public void CheckLine_ContainsNoSpaceForKeyword_AddsError(string lineContent, string keyword)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 9
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0022), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0022.Replace("{keyword}", keyword), line.Errors[0].Message);
    }

    [Theory]
    [InlineData("new ()", "new")]
    [InlineData("new    ()", "new")]
    [InlineData("    new (\"test\")", "new")]
    public void CheckLine_ContainsSpacesBeforeParenthesisForKeyword_AddsError(string lineContent, string keyword)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 10
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0023), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0023.Replace("{keyword}", keyword), line.Errors[0].Message);
    }

    [Theory]
    [InlineData("    if  (a == b)", "if")]
    [InlineData("    for  (int i=0; i<total; i++)", "for")]
    [InlineData("    foreach  (var a in list)", "foreach")]
    [InlineData("    do  (a > b)", "do")]
    [InlineData("    while  (a > b)", "while")]
    public void CheckLine_ContainsMoreThanOneSpaceBeforeParenthesisForKeyword_AddsError(string lineContent, string keyword)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 11
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0024), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0024.Replace("{keyword}", keyword), line.Errors[0].Message);
    }

    [Theory]
    [InlineData("    /// <summary></summary>")]
    [InlineData("    /// <returns></returns>")]
    public void CheckLine_ContainsForbiddenDocumentation_AddsError(string lineContent)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 12
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0030), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0030, line.Errors[0].Message);
    }

    [Theory]
    [InlineData("    ///Uses this.Value -", nameof(Errors.CH0020))]
    [InlineData("    /// <returns></returns> this.Value -", nameof(Errors.CH0030))]
    public void CheckLine_DocumentationError_StopsBeforeCodeRules(string lineContent, string expectedCode)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 1
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert: no CH0018 (ends with "-") or CH0019 ("this.") for a documentation line
        Assert.Equal(expectedCode, Assert.Single(line.Errors).Code);
    }

    [Theory]
    [InlineData("    if (a != b) // some comment")]
    [InlineData("    if (a != b) // Done?")]
    [InlineData("    var x = 1; // Same as this.Value")]
    [InlineData("    var x = y; // a + b -")]
    public void CheckLine_TrailingComment_IsNotCheckedAsCode(string lineContent)
    {
        // Arrange
        LineModel line = new()
        {
            Content = lineContent,
            Number = 1,
            PreviousIndentationLevel = 1
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void CheckLine_TrailingComment_DoesNotHideForbiddenEndToken()
    {
        // Arrange
        LineModel line = new()
        {
            Content = "    var x = a + // Why",
            Number = 1,
            PreviousIndentationLevel = 1
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0018), error.Code);
        Assert.Equal(Errors.CH0018.Replace("{token}", "+"), error.Message);
    }

    [Fact]
    public void CheckLine_TrailingCommentWithoutSpace_AddsCh0021()
    {
        // Arrange
        LineModel line = new()
        {
            Content = "    if (a != b) //some comment",
            Number = 1,
            PreviousIndentationLevel = 1
        };

        // Act
        _lineChecker.CheckLine(line);

        // Assert
        Assert.Equal(nameof(Errors.CH0021), Assert.Single(line.Errors).Code);
    }
}
