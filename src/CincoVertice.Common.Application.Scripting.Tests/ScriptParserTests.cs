using CincoVertice.Common.Application.Scripting.Ast;
using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class ScriptParserTests
{
    private static IReadOnlyList<Statement> Parse(string source) =>
        new ScriptParser().Parse(ScriptLexer.Tokenize(source));

    [Theory]
    [InlineData("x = 1 + 2 * 3", "x = Add(1, Multiply(2, 3))")]
    [InlineData("x = (1 + 2) * 3", "x = Multiply(Add(1, 2), 3)")]
    [InlineData("x = 1 - 2 - 3", "x = Subtract(Subtract(1, 2), 3)")]
    [InlineData("x = 8 / 4 % 3", "x = Modulo(Divide(8, 4), 3)")]
    [InlineData("x = -2 + 5", "x = Add(Negate(2), 5)")]
    [InlineData("x = --2", "x = Negate(Negate(2))")]
    [InlineData("x = 1 + 2 < 4", "x = Less(Add(1, 2), 4)")]
    [InlineData("x = a or b and c", "x = Or(a, And(b, c))")]
    [InlineData("x = not a == b", "x = Not(Equal(a, b))")]
    [InlineData("x = not not a and b", "x = And(Not(Not(a)), b)")]
    [InlineData("x = a == b or c != d", "x = Or(Equal(a, b), NotEqual(c, d))")]
    public void Parse_FollowsPrecedence(string source, string expected)
    {
        Assert.Equal(expected, AstPrinter.Print(Parse(source)));
    }

    [Theory]
    [InlineData("x = \"a\"", "x = \"a\"")]
    [InlineData("x = true", "x = true")]
    [InlineData("x = 2.50", "x = 2.50")]
    [InlineData("x = [1, \"a\", [true]]", "x = [1, \"a\", [true]]")]
    [InlineData("x = []", "x = []")]
    [InlineData("x = list[1][2]", "x = Index(Index(list, 1), 2)")]
    [InlineData("x = f(1)[2]", "x = Index(f(1), 2)")]
    [InlineData("x = [1, 2][1 + 1]", "x = Index([1, 2], Add(1, 1))")]
    [InlineData("x = clipboard.gettext()", "x = clipboard.gettext()")]
    [InlineData("say(text.upper(name), 2)", "say(text.upper(name), 2)")]
    [InlineData("x = 1\ny = x", "x = 1; y = x")]
    public void Parse_ReadsValuesCallsListsAndIndexes(string source, string expected)
    {
        Assert.Equal(expected, AstPrinter.Print(Parse(source)));
    }

    [Fact]
    public void Parse_ReadsIfWithElseifAndElse()
    {
        // Arrange
        string source =
            "if x < 10\n"
            + "    say(\"small\")\n"
            + "elseif x < 20\n"
            + "    say(\"medium\")\n"
            + "    y = 2\n"
            + "else\n"
            + "    if z\n"
            + "    end\n"
            + "end";

        // Act
        string printed = AstPrinter.Print(Parse(source));

        // Assert
        Assert.Equal(
            "if Less(x, 10) { say(\"small\") } elseif Less(x, 20) { say(\"medium\"); y = 2 } else { if z {  } }",
            printed);
    }

    [Fact]
    public void Parse_KeepsTheLineAndColumnOfEachNode()
    {
        // Act
        var assignment = (AssignmentStatement)Parse("\n  x = a + f(b)")[0];

        // Assert
        var sum = (BinaryExpression)assignment.Value;
        var call = (CallExpression)sum.Right;
        Assert.Equal((2, 3), (assignment.Line, assignment.Column));
        Assert.Equal((2, 9), (sum.Line, sum.Column));
        Assert.Equal((2, 11), (call.Line, call.Column));
        Assert.Equal((2, 13), (call.Arguments[0].Line, call.Arguments[0].Column));
    }

    [Theory]
    [InlineData("x == 1", 1, 1, "A line must assign")]
    [InlineData("if x = 1\nend", 1, 6, "Use == to compare")]
    [InlineData("if true\n    say(1)", 2, 11, "Missing 'end' for the 'if' on line 1")]
    [InlineData("end", 1, 1, "'end' without an 'if'")]
    [InlineData("say(1, 2", 1, 9, "Missing ')'")]
    [InlineData("x = [1, 2", 1, 10, "Missing ']' to close the list")]
    [InlineData("x = ", 1, 5, "Expected a value")]
    [InlineData("x = 1 < 2 < 3", 1, 11, "two values at a time")]
    [InlineData("if true then\nend", 1, 9, "end of the line")]
    [InlineData("x[1] = 2", 1, 6, "A list's items can't be changed")]
    [InlineData("my.name = 1", 1, 1, "A variable name can't contain '.'")]
    public void Parse_ReportsSyntaxErrorsWithPosition(string source, int line, int column, string message)
    {
        // Act
        var error = Assert.Throws<ScriptException>(() => Parse(source));

        // Assert
        Assert.Equal((line, column), (error.Line, error.Column));
        Assert.Contains(message, error.Message);
    }

    [Fact]
    public void Parse_WhenTokensDontEndWithEndOfScript_Throws()
    {
        Assert.Throws<ArgumentException>(() => new ScriptParser().Parse([]));
    }
}
