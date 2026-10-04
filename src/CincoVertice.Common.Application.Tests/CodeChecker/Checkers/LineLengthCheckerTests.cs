using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class LineLengthCheckerTests
{
    private readonly LineLengthChecker _checker = new();

    [Fact]
    public void MaxLength_Is120()
    {
        Assert.Equal(120, LineLengthChecker.MaxLength);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(119)]
    [InlineData(120)]
    public void Check_WhenLineIsWithinMaxLength_AddsNoError(int length)
    {
        var line = TestLine.Create(new string('a', length));

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData(121)]
    [InlineData(200)]
    public void Check_WhenLineIsLongerThanMaxLength_AddsError(int length)
    {
        var line = TestLine.Create(new string('a', length), number: 9);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0016), error.Code);
        Assert.Equal(Errors.CH0016, error.Message);
        Assert.Equal(9, error.Line);
    }

    [Fact]
    public void Check_CountsIndentation()
    {
        // 8 spaces + 113 characters = 121
        var line = TestLine.Create(new string(' ', 8) + new string('a', 113));

        _checker.Check(line);

        Assert.Equal(nameof(Errors.CH0016), Assert.Single(line.Errors).Code);
    }
}
