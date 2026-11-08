namespace CincoVertice.Common.Application.Scripting.Ast;

public record NumberExpression(decimal Value, int Line, int Column) : Expression(Line, Column);
