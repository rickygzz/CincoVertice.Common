namespace CincoVertice.Common.Application.Scripting.Ast;

/// <param name="Line">Line of the operator.</param>
/// <param name="Column">Column of the operator.</param>
public record BinaryExpression(BinaryOperator Operator, Expression Left, Expression Right, int Line, int Column)
    : Expression(Line, Column);
