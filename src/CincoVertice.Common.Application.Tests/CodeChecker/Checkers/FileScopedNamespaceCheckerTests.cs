using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers;

public class FileScopedNamespaceCheckerTests
{
    private readonly FileScopedNamespaceChecker _checker = new();

    [Theory]
    [InlineData("namespace CincoVertice.Common;")]
    [InlineData("namespace My_App.V2;")]
    [InlineData("var namespaceName = name;")]
    [InlineData("string namespace = \"x\";")]
    [InlineData("{")]
    public void Check_WhenLineIsNotBlockNamespace_ContinuesWithoutError(string content)
    {
        var line = TestLine.Create(content);

        var result = _checker.Check(line);

        Assert.True(result);
        Assert.Empty(line.Errors);
    }

    [Theory]
    [InlineData("namespace CincoVertice.Common")]
    [InlineData("namespace CincoVertice.Common {")]
    [InlineData("namespace CincoVertice.Common{")]
    [InlineData("    namespace Nested")]
    public void Check_WhenNamespaceIsBlock_AddsCh0029AndContinues(string content)
    {
        var line = TestLine.Create(content, number: 3);

        var result = _checker.Check(line);

        Assert.True(result);
        var error = Assert.Single(line.Errors);
        Assert.Equal(nameof(Errors.CH0029), error.Code);
        Assert.Equal(Errors.CH0029, error.Message);
        Assert.Equal(3, error.Line);
    }
}
