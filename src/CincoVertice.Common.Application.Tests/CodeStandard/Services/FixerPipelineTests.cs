using CincoVertice.Common.Application.CodeStandard.Fixers;
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
