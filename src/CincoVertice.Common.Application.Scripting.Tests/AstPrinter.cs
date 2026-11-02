using System.Globalization;
using CincoVertice.Common.Application.Scripting.Ast;

namespace CincoVertice.Common.Application.Scripting.Tests;

/// <summary>
///     Writes parsed statements in a short form for tests, so the shape of the tree is easy to compare:
///     x = Add(1, Multiply(2, 3)); say("a"); if Less(x, 1) { y = 1 } elseif ... else { ... }
///     <para>Calls are written name(arguments), lists [items], list[index] as Index(list, index), and texts in
///     quotes.</para>
/// </summary>
internal static class AstPrinter
{
    public static string Print(IReadOnlyList<Statement> statements) => string.Join("; ", statements.Select(Print));

    private static string Print(Statement statement) => statement switch
    {
        AssignmentStatement assignment => $"{assignment.Name} = {Print(assignment.Value)}",
        CallStatement call => Print(call.Call),
        IfStatement ifStatement => PrintIf(ifStatement),
        _ => throw new ArgumentException($"Unknown statement {statement.GetType().Name}", nameof(statement)),
    };

    private static string PrintIf(IfStatement ifStatement)
    {
        var branches = ifStatement.Branches.Select((branch, i) =>
            $"{(i == 0 ? "if" : "elseif")} {Print(branch.Condition)} {{ {Print(branch.Body)} }}");
        string text = string.Join(" ", branches);

        return ifStatement.Else.Count > 0 ? $"{text} else {{ {Print(ifStatement.Else)} }}" : text;
    }

    private static string Print(Expression expression) => expression switch
    {
        NumberExpression number => number.Value.ToString(CultureInfo.InvariantCulture),
        TextExpression text => $"\"{text.Value}\"",
        BooleanExpression boolean => boolean.Value ? "true" : "false",
        VariableExpression variable => variable.Name,
        UnaryExpression unary => $"{unary.Operator}({Print(unary.Operand)})",
        BinaryExpression binary => $"{binary.Operator}({Print(binary.Left)}, {Print(binary.Right)})",
        CallExpression call => $"{call.Name}({string.Join(", ", call.Arguments.Select(Print))})",
        ListExpression list => $"[{string.Join(", ", list.Items.Select(Print))}]",
        IndexExpression index => $"Index({Print(index.List)}, {Print(index.Index)})",
        _ => throw new ArgumentException($"Unknown expression {expression.GetType().Name}", nameof(expression)),
    };
}
