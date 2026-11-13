using CincoVertice.Common.Application.CodeStandard.Fixers;
using CincoVertice.Common.Application.CodeStandard.Fixers.CSharp;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Services;

public class FixerPipelineTests
{
    [Fact]
    public void Run_RunsFixersInOrder()
    {
        // Arrange
        var pipeline = new FixerPipeline([new CommentSpaceFixer(), new TrailingWhitespaceFixer()]);

        // Act
        string result = pipeline.Run("x(); //a   \n");

        // Assert
        Assert.Equal("x(); // a\n", result);
    }

    [Fact]
    public void Names_AreTheFixersClassNamesWithoutFixer_InTheirOrder()
    {
        // Arrange
        var pipeline = new FixerPipeline([new CommentSpaceFixer(), new TrailingWhitespaceFixer()]);

        // Act and assert
        Assert.Equal(["CommentSpace", "TrailingWhitespace"], pipeline.Names);
    }

    [Fact]
    public void Only_RunsTheFixersNamed_AndIgnoresNamesItDoesntHave()
    {
        // Arrange
        var pipeline = new FixerPipeline([new CommentSpaceFixer(), new TrailingWhitespaceFixer()]);

        // Act
        FixerPipeline only = pipeline.Only(["TrailingWhitespace", "BraceIndentation"]);

        // Assert: the comment keeps its missing space
        Assert.Equal(["TrailingWhitespace"], only.Names);
        Assert.Equal("x(); //a\n", only.Run("x(); //a   \n"));
    }

    [Fact]
    public void EveryFixerOfEveryLanguage_HasANameOfItsOwn()
    {
        // Act
        IEnumerable<string> names = CodeLanguageModel.All.SelectMany(language => language.Fixers.Names);

        // Assert: one name per fixer within a language; a name never ends in "Fixer"
        Assert.All(CodeLanguageModel.All, language =>
            Assert.Equal(language.Fixers.Names.Count, language.Fixers.Names.Distinct().Count()));
        Assert.DoesNotContain(names, name => name.EndsWith("Fixer", StringComparison.Ordinal));
    }

    [Fact]
    public void Run_FixesCodeButNotTextInsideLiterals()
    {
        // Arrange
        var pipeline = new FixerPipeline([new KeywordParenthesisSpaceFixer(), new TrailingWhitespaceFixer()]);

        // Act
        string result = pipeline.Run("if(x) { s = \"if(y)  \"; }   ");

        // Assert
        Assert.Equal("if (x) { s = \"if(y)  \"; }", result);
    }

    [Fact]
    public void CreateFormatting_LeavesMultiLineVerbatimStringUnchanged()
    {
        // Arrange
        string content =
            "public class A\n"
            + "{\n"
            + "    private string _s = @\"line   \n"
            + "  indented\n"
            + "\n"
            + "\n"
            + "\";\n"
            + "}\n";

        // Act
        string result = FixerPipeline.CreateFormatting().Run(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void CreateFormatting_IsIdempotent()
    {
        // Arrange
        var pipeline = FixerPipeline.CreateFormatting();
        string once = pipeline.Run(
            "namespace A\n{\n    public class B\n    {\n        //c   \n\n\n"
            + "        public int X(int a) { if(a > 0) return 1; return 0; }\n    }\n}");

        // Act
        string twice = pipeline.Run(once);

        // Assert
        Assert.Equal(once, twice);
    }
}
