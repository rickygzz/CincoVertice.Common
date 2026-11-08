namespace CincoVertice.Common.Application.Calculator.Ast;

/// <param name="Position">Position of the function name.</param>
public record FunctionCallNode(string Name, IReadOnlyList<ExpressionNode> Arguments, int Position)
    : ExpressionNode(Position)
{
    public override string ToString() =>
        Arguments.Count == 0 ? $"({Name})" : $"({Name} {string.Join(" ", Arguments)})";
}
