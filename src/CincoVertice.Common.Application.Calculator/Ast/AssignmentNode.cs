namespace CincoVertice.Common.Application.Calculator.Ast;

/// <summary>A value given a name, to use in later calculations: x = 5.</summary>
public record AssignmentNode(string Name, ExpressionNode Value, int Position) : ExpressionNode(Position)
{
    public override string ToString() => $"(= {Name} {Value})";
}
