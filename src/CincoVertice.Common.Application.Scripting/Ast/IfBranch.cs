namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>The if or one elseif of an <see cref="IfStatement"/>: runs Body when Condition is true.</summary>
public record IfBranch(Expression Condition, IReadOnlyList<Statement> Body);
