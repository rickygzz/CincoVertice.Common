namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>An item of a list: list[index], where the first item is 1.</summary>
/// <param name="Line">Line of the [.</param>
/// <param name="Column">Column of the [.</param>
public record IndexExpression(Expression List, Expression Index, int Line, int Column) : Expression(Line, Column);
