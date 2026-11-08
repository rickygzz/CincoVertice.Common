namespace CincoVertice.Common.Application.Scripting.Ast;

public record UnaryExpression(UnaryOperator Operator, Expression Operand, int Line, int Column)
    : Expression(Line, Column);
