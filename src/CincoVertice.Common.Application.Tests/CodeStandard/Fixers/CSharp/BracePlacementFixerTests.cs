using CincoVertice.Common.Application.CodeStandard.Fixers.CSharp;
using CincoVertice.Common.Application.CodeStandard.Services;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers.CSharp;

public class BracePlacementFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new BracePlacementFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData("\n")]
    [InlineData("\r\n")]
    public void Fix_SplitsIfElseBracesAndKeepsLineEndings(string nl)
    {
        // Arrange
        string content =
            "        if (Arrays.Equals(ace.certVerifier, certVerifier)) {" + nl
            + "            SetSecretKey(secretKey);" + nl
            + "            return true;" + nl
            + "        } else {" + nl
            + "            return false;" + nl
            + "        }" + nl;

        // Act
        var result = new BracePlacementFixer().Fix(content);

        // Assert
        Assert.Equal(
            "        if (Arrays.Equals(ace.certVerifier, certVerifier))" + nl
            + "        {" + nl
            + "            SetSecretKey(secretKey);" + nl
            + "            return true;" + nl
            + "        }" + nl
            + "        else" + nl
            + "        {" + nl
            + "            return false;" + nl
            + "        }" + nl,
            result);
    }

    [Theory]
    [InlineData("while (a > b) {", "while (a > b)\n{")]
    [InlineData("    if (a > b)  {", "    if (a > b)\n    {")]
    [InlineData("    } else if (a > b) {", "    }\n    else if (a > b)\n    {")]
    [InlineData("    } if (a > b)", "    }\n    if (a > b)")]
    [InlineData("} catch (Exception ex) {", "}\ncatch (Exception ex)\n{")]
    [InlineData("} finally {", "}\nfinally\n{")]
    [InlineData("public void Run() {", "public void Run()\n{")]
    [InlineData("public class Foo : Bar {", "public class Foo : Bar\n{")]
    [InlineData("list.ForEach(x => {", "list.ForEach(x =>\n{")]
    [InlineData("if (x) { // why", "if (x) // why\n{")]
    public void Fix_PutsBlockBracesOnTheirOwnLines(string content, string expected)
    {
        // Act
        var result = new BracePlacementFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{\n    A();\n}\n")]
    [InlineData("} while (a > b);\n")]
    [InlineData("});\n")]
    [InlineData("},\n")]
    [InlineData("}.Run();\n")]
    [InlineData("get { return x; }\n")]
    [InlineData("if (x) { return; }\n")]
    [InlineData("var x = 1; // {\n")]
    [InlineData("/*\n} else {\n*/\n")]
    [InlineData("var s = @\"{\n} else {\";\n")]
    public void Fix_LeavesOwnLineBracesPunctuationCommentsAndLiteralsUnchanged(string content)
    {
        // Act
        var result = new BracePlacementFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        var fixer = new BracePlacementFixer();
        string once = fixer.Fix("try {\n    A();\n} catch {\n    B();\n} finally {\n    C();\n}\n");

        // Act
        string twice = fixer.Fix(once);

        // Assert
        Assert.Equal(once, twice);
    }

    [Fact]
    public void CreateFormatting_ConvertsJavaStyleIfElseInCrlfFile()
    {
        // Arrange
        string content =
            "class A\r\n{\r\n    bool Run()\r\n    {\r\n"
            + "        if (Arrays.Equals(ace.certVerifier, certVerifier)) {\r\n"
            + "            return true;\r\n"
            + "        } else {\r\n"
            + "            return false;\r\n"
            + "        }\r\n"
            + "    }\r\n}\r\n";

        // Act
        string result = FixerPipeline.CreateFormatting().Run(content);

        // Assert
        Assert.Equal(
            "class A\r\n{\r\n    bool Run()\r\n    {\r\n"
            + "        if (Arrays.Equals(ace.certVerifier, certVerifier))\r\n"
            + "        {\r\n"
            + "            return true;\r\n"
            + "        }\r\n"
            + "        else\r\n"
            + "        {\r\n"
            + "            return false;\r\n"
            + "        }\r\n"
            + "    }\r\n}\r\n",
            result);
    }
}
