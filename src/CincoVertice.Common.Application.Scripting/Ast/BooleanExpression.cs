namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>true or false.</summary>
public record BooleanExpression(bool Value, int Line, int Column) : Expression(Line, Column);
