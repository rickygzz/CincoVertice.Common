using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class IndentationCheckerTests
{
    private readonly IndentationChecker _checker = new();

    [Fact]
    public void Check_WhenLineHasNoIndentation_SetsIndentationLevelToZero()
    {
        var line = new LineModel
        {
            Content = "public class Test",
            PreviousIndentationLevel = 0
        };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(0, line.IndentationLevel);
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void Check_WhenLineHasFourSpaces_SetsIndentationLevelToOne()
    {
        var line = new LineModel
        {
            Content = "    public class Test",
            PreviousIndentationLevel = 0
        };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(1, line.IndentationLevel);
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void Check_WhenLineHasEightSpaces_SetsIndentationLevelToTwo()
    {
        var line = new LineModel
        {
            Content = "        public class Test",
            PreviousIndentationLevel = 1
        };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(2, line.IndentationLevel);
        Assert.Empty(line.Errors);
    }

    [Fact]
    public void Check_WhenLineHasTabs_AddsCh0012Error()
    {
        var line = new LineModel
        {
            Content = "\tpublic class Test",
            PreviousIndentationLevel = 0
        };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(1, line.IndentationLevel);
        Assert.Contains(line.Errors, e => e.Code == nameof(Errors.CH0012));
    }

    [Fact]
    public void Check_WhenLineHasThreeSpaces_AddsCh0013Error()
    {
        var line = new LineModel
        {
            Content = "   public class Test",
            PreviousIndentationLevel = 0
        };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(1, line.IndentationLevel);
        Assert.Contains(line.Errors, e => e.Code == nameof(Errors.CH0013));
    }

    [Fact]
    public void Check_WhenLineHasMixedTabsAndSpaces_AddsCh0012Error()
    {
        var line = new LineModel
        {
            Content = " \tpublic class Test",
            PreviousIndentationLevel = 0
        };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(1, line.IndentationLevel);
        Assert.Contains(line.Errors, e => e.Code == nameof(Errors.CH0012));
    }

    [Fact]
    public void Check_WhenIndentationJumpsMoreThanOneLevel_AddsCh0014Error()
    {
        var line = new LineModel
        {
            Content = "            public class Test", // 12 spaces = level 3
            PreviousIndentationLevel = 1
        };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(3, line.IndentationLevel);
        Assert.Contains(line.Errors, e => e.Code == nameof(Errors.CH0014));
    }

    [Fact]
    public void Check_WhenIndentationIncreasesByOneLevel_DoesNotAddCh0014Error()
    {
        var line = new LineModel
        {
            Content = "        public class Test", // 8 spaces = level 2
            PreviousIndentationLevel = 1
        };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(2, line.IndentationLevel);
        Assert.DoesNotContain(line.Errors, e => e.Code == nameof(Errors.CH0014));
    }

    [Fact]
    public void Check_WhenIndentationDecreases_DoesNotAddCh0014Error()
    {
        var line = new LineModel
        {
            Content = "    public class Test", // level 1
            PreviousIndentationLevel = 3
        };

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Equal(1, line.IndentationLevel);
        Assert.Empty(line.Errors);
    }
}
