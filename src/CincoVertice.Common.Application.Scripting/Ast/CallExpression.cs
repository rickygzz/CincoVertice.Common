namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>A function call: name(argument, ...).</summary>
public record CallExpression(string Name, IReadOnlyList<Expression> Arguments, int Line, int Column)
    : Expression(Line, Column);
