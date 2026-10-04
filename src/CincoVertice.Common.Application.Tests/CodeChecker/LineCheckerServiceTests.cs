using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVerticeCommon.Application.CodeChecker;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker;

public class CodeCheckerTest

{
    private readonly CodeCheckerService _checker;

    public CodeCheckerTest()
    {
        _checker = new CodeCheckerService();
    }

    [Fact]
    public void CheckFile_FileNotFound_AddsError()
    {
        // Arrange
        string filePath = "non-existent_file.txt";

        // Act
        _checker.CheckFile(filePath, out List<ErrorModel> errors);

        // Assert
        Assert.Single(errors);
        Assert.Equal(nameof(Errors.CH0001), errors[0].Code);
        Assert.Equal(0, errors[0].Line);
        Assert.Equal(Errors.CH0001, errors[0].Message);
    }

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

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
        CodeCheckerService.CheckLine(line);

        // Assert
        Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0030), line.Errors[0].Code);
        Assert.Equal(line.Number, line.Errors[0].Line);
        Assert.Equal(Errors.CH0030, line.Errors[0].Message);
    }
}
