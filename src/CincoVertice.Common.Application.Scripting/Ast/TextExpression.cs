namespace CincoVertice.Common.Application.Scripting.Ast;

public record TextExpression(string Value, int Line, int Column) : Expression(Line, Column);
