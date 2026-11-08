namespace CincoVertice.Common.Application.Calculator.Ast;

/// <param name="Position">Position of the operator.</param>
public record UnaryNode(UnaryOperator Operator, ExpressionNode Operand, int Position) : ExpressionNode(Position)
{
    public override string ToString() => $"({Operator.ToSymbol()} {Operand})";
}
