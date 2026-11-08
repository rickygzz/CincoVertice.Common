using CincoVertice.Common.Application.CodeStandard.Fixers.CSharp;
using CincoVertice.Common.Application.CodeStandard.Services;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers.CSharp;

public class ParenthesisSpaceFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new ParenthesisSpaceFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData(
        "    internal void Read( LittleEndianByteArrayInputStream lei )\n",
        "    internal void Read(LittleEndianByteArrayInputStream lei)\n")]
    [InlineData("Foo( a, ( b + c ) );\n", "Foo(a, (b + c));\n")]
    [InlineData("Run( );\n", "Run();\n")]
    [InlineData("if ( x )\r\n", "if (x)\r\n")]
    [InlineData("Foo(\ta\t);\n", "Foo(a);\n")]
    public void Fix_RemovesSpacesInsideParentheses(string content, string expected)
    {
        // Act
        var result = new ParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("Foo(\n    a,\n    b\n    );\n")]
    [InlineData("Foo(   \n    a);\n")]
    [InlineData("Foo( // why\n    a);\n")]
    [InlineData("Foo( /* a */ b);\n")]
    [InlineData("var s = \"( a )\";\n")]
    [InlineData("var c = ' ';\n")]
    [InlineData("x = 1; // ( a )\n")]
    [InlineData("/* ( a ) */\n")]
    public void Fix_LeavesLineEdgesCommentsAndLiteralsUnchanged(string content)
    {
        // Act
        var result = new ParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_RemovesSpacesInsideParenthesesOfMethodBody()
    {
        // Arrange
        string content =
            "    internal int Write( Stream out1 )\r\n"
            + "    {\r\n"
            + "        LittleEndian.PutUInt( _value.Length / 2, out1 );\r\n"
            + "        out1.Write( _value, 0, _value.Length );\r\n"
            + "        return LittleEndianConsts.INT_SIZE + _value.Length;\r\n"
            + "    }\r\n";

        // Act
        var result = new ParenthesisSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(
            "    internal int Write(Stream out1)\r\n"
            + "    {\r\n"
            + "        LittleEndian.PutUInt(_value.Length / 2, out1);\r\n"
            + "        out1.Write(_value, 0, _value.Length);\r\n"
            + "        return LittleEndianConsts.INT_SIZE + _value.Length;\r\n"
            + "    }\r\n",
            result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        var fixer = new ParenthesisSpaceFixer();
        string once = fixer.Fix("Foo( a, ( b ) );\n");

        // Act
        string twice = fixer.Fix(once);

        // Assert
        Assert.Equal("Foo(a, (b));\n", once);
        Assert.Equal(once, twice);
    }

    [Fact]
    public void CreateFormatting_ConvertsJavaStyleTryCatchAndMethodSignature()
    {
        // Arrange
        string content =
            "class A\r\n{\r\n"
            + "    internal void Read( LittleEndianByteArrayInputStream lei )\r\n"
            + "    {\r\n"
            + "        try {\r\n"
            + "            hashFinal = cipher.DoFinal(inputKey);\r\n"
            + "        } catch (Exception e) {\r\n"
            + "            throw new EncryptedDocumentException(e);\r\n"
            + "        }\r\n"
            + "    }\r\n}\r\n";

        // Act
        string result = FixerPipeline.CreateFormatting().Run(content);

        // Assert
        Assert.Equal(
            "class A\r\n{\r\n"
            + "    internal void Read(LittleEndianByteArrayInputStream lei)\r\n"
            + "    {\r\n"
            + "        try\r\n"
            + "        {\r\n"
            + "            hashFinal = cipher.DoFinal(inputKey);\r\n"
            + "        }\r\n"
            + "        catch (Exception e)\r\n"
            + "        {\r\n"
            + "            throw new EncryptedDocumentException(e);\r\n"
            + "        }\r\n"
            + "    }\r\n}\r\n",
            result);
    }
}
