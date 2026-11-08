namespace CincoVertice.Common.Application.Calculator.Ast;

public static class OperatorExtensions
{
    public static string ToSymbol(this UnaryOperator op) => op switch
    {
        UnaryOperator.Plus => "+",
        UnaryOperator.Negate => "-",
        _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
    };

    public static string ToSymbol(this BinaryOperator op) => op switch
    {
        BinaryOperator.Add => "+",
        BinaryOperator.Subtract => "-",
        BinaryOperator.Multiply => "*",
        BinaryOperator.Divide => "/",
        BinaryOperator.Modulo => "%",
        BinaryOperator.Power => "^",
        _ => throw new ArgumentOutOfRangeException(nameof(op), op, null)
    };
}
