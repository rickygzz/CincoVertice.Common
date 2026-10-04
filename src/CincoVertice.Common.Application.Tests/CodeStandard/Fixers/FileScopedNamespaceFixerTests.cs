using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class FileScopedNamespaceFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new FileScopedNamespaceFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Fact]
    public void Fix_WhenEmptyNamespaceBraceOnSameLine_ConvertsToFileScoped()
    {
        // Arrange
        string content = "namespace CincoVertice.Common.Services {\r\n}";

        // Act
        var result = new FileScopedNamespaceFixer().Fix(content);

        // Assert
        Assert.Equal("namespace CincoVertice.Common.Services;\r\n", result);
    }

    [Fact]
    public void Fix_WhenEmptyNamespaceBraceOnNextLine_ConvertsToFileScoped()
    {
        // Arrange
        string content = "namespace Foo\r\n{\r\n}";

        // Act
        var result = new FileScopedNamespaceFixer().Fix(content);

        // Assert
        Assert.Equal("namespace Foo;\r\n", result);
    }

    [Fact]
    public void Fix_WhenNamespaceHasBody_DeindentsContentAndInsertsBlankLine()
    {
        // Arrange
        string content =
            "namespace Foo\r\n" +
            "{\r\n" +
            "    public class X\r\n" +
            "    {\r\n" +
            "    }\r\n" +
            "}";

        // Act
        var result = new FileScopedNamespaceFixer().Fix(content);

        // Assert
        Assert.Equal(
            "namespace Foo;\r\n" +
            "\r\n" +
            "public class X\r\n" +
            "{\r\n" +
            "}\r\n",
            result);
    }

    [Fact]
    public void Fix_WhenBraceOnSameLineWithBody_DeindentsContent()
    {
        // Arrange
        string content =
            "namespace Foo {\r\n" +
            "    class X\r\n" +
            "    {\r\n" +
            "    }\r\n" +
            "}";

        // Act
        var result = new FileScopedNamespaceFixer().Fix(content);

        // Assert
        Assert.Equal(
            "namespace Foo;\r\n" +
            "\r\n" +
            "class X\r\n" +
            "{\r\n" +
            "}\r\n",
            result);
    }

    [Fact]
    public void Fix_PreservesUsingsBeforeNamespace()
    {
        // Arrange
        string content =
            "using System;\r\n" +
            "\r\n" +
            "namespace Foo\r\n" +
            "{\r\n" +
            "    class X { }\r\n" +
            "}";

        // Act
        var result = new FileScopedNamespaceFixer().Fix(content);

        // Assert
        Assert.Equal(
            "using System;\r\n" +
            "\r\n" +
            "namespace Foo;\r\n" +
            "\r\n" +
            "class X { }\r\n",
            result);
    }

    [Fact]
    public void Fix_DeindentsNestedContentByOneLevel()
    {
        // Arrange
        string content =
            "namespace Foo\r\n" +
            "{\r\n" +
            "    class X\r\n" +
            "    {\r\n" +
            "        int y;\r\n" +
            "    }\r\n" +
            "}";

        // Act
        var result = new FileScopedNamespaceFixer().Fix(content);

        // Assert
        Assert.Equal(
            "namespace Foo;\r\n" +
            "\r\n" +
            "class X\r\n" +
            "{\r\n" +
            "    int y;\r\n" +
            "}\r\n",
            result);
    }

    [Fact]
    public void Fix_PreservesLfLineEndings()
    {
        // Arrange
        string content = "namespace Foo\n{\n    class X\n}";

        // Act
        var result = new FileScopedNamespaceFixer().Fix(content);

        // Assert
        Assert.Equal("namespace Foo;\n\nclass X\n", result);
    }

    [Fact]
    public void Fix_WhenAlreadyFileScoped_LeavesUnchanged()
    {
        // Arrange
        string content = "namespace Foo;\n\npublic class X\r\n{\r\n}";

        // Act
        var result = new FileScopedNamespaceFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_WhenNoNamespace_LeavesUnchanged()
    {
        // Arrange
        string content = "public class X\r\n{\r\n}";

        // Act
        var result = new FileScopedNamespaceFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }
}
