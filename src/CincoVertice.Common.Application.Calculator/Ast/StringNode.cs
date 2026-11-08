namespace CincoVertice.Common.Application.Calculator.Ast;

/// <summary>Text literal. Only valid as a function argument, e.g. len("abc").</summary>
public record StringNode(string Value, int Position) : ExpressionNode(Position)
{
    public override string ToString() => $"\"{Value.Replace("\"", "\"\"")}\"";
}
