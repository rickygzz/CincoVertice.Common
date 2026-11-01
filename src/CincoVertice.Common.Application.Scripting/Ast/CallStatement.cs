namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>A function call on its own line, such as say("Hi"); its result is not used.</summary>
public record CallStatement(CallExpression Call, int Line, int Column) : Statement(Line, Column);
