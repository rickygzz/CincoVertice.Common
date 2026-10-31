using CincoVertice.Common.Application.CodeStandard.Fixers;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Fixers;

public class BraceSpaceFixerTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void Fix_WhenContentIsNullOrEmpty_ReturnsEmpty(string? content)
    {
        // Act
        var result = new BraceSpaceFixer().Fix(content!);

        // Assert
        Assert.Equal(string.Empty, result);
    }

    [Theory]
    [InlineData(
        "    get{return (byte)((RgbColor >> 8) & 0xFF);}\n",
        "    get { return (byte)((RgbColor >> 8) & 0xFF); }\n")]
    [InlineData("public int X {get; set;}\n", "public int X { get; set; }\n")]
    [InlineData("var a = new[] {1, 2};\n", "var a = new[] { 1, 2 };\n")]
    [InlineData("void Run() {}\n", "void Run() { }\n")]
    [InlineData("var p = new {A = new {B = 1}};\n", "var p = new { A = new { B = 1 } };\n")]
    [InlineData("Foo(() =>{x();});\n", "Foo(() => { x(); });\n")]
    [InlineData("if (o is {Length: 0}) { }\n", "if (o is { Length: 0 }) { }\n")]
    public void Fix_AddsSpacesAroundAndInsideBraces(string content, string expected)
    {
        // Act
        var result = new BraceSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{\n    return;\n}\n")]
    [InlineData("    {\r\n        A();\r\n    }\r\n")]
    [InlineData("var s = $\"{a}{b}\";\n")]
    [InlineData("public override string ToString() => $\"\\\"{Value.Replace(\"\\\"\", \"\\\"\\\"\")}\\\"\";\n")]
    [InlineData("var c = '{';\n")]
    [InlineData("var x = 1; // {a}\n")]
    [InlineData("/// <see cref=\"List{T}\"/>\n")]
    [InlineData("Foo([{ A = 1 }]);\n")]
    [InlineData("Foo({ A = 1 });\n")]
    [InlineData("var x = new { A = 1 };\n")]
    public void Fix_LeavesSpacedBracesLiteralsAndCommentsUnchanged(string content)
    {
        // Act
        var result = new BraceSpaceFixer().Fix(content);

        // Assert
        Assert.Equal(content, result);
    }

    [Fact]
    public void Fix_IsIdempotent()
    {
        // Arrange
        var fixer = new BraceSpaceFixer();
        string once = fixer.Fix("get{return x;}\nset{}\n");

        // Act
        string twice = fixer.Fix(once);

        // Assert
        Assert.Equal("get { return x; }\nset { }\n", once);
        Assert.Equal(once, twice);
    }
}
