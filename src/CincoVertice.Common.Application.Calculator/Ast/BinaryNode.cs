namespace CincoVertice.Common.Application.Calculator.Ast;

/// <param name="Position">Position of the operator.</param>
public record BinaryNode(BinaryOperator Operator, ExpressionNode Left, ExpressionNode Right, int Position)
    : ExpressionNode(Position)
{
    public override string ToString() => $"({Operator.ToSymbol()} {Left} {Right})";
}
