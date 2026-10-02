namespace CincoVertice.Common.Application.Calculator.Ast;

/// <summary>A named value, such as the constant pi.</summary>
public record IdentifierNode(string Name, int Position) : ExpressionNode(Position)
{
    public override string ToString() => Name;
}
