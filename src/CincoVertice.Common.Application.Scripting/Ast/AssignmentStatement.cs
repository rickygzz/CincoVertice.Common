namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>name = value</summary>
public record AssignmentStatement(string Name, Expression Value, int Line, int Column) : Statement(Line, Column);
