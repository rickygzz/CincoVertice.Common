namespace CincoVertice.Common.Application.Scripting.Ast;

public record VariableExpression(string Name, int Line, int Column) : Expression(Line, Column);
