namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>A list literal: [item, ...].</summary>
public record ListExpression(IReadOnlyList<Expression> Items, int Line, int Column) : Expression(Line, Column);
