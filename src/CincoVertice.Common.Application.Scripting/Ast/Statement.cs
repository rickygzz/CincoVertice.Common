namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>
///     One step of a script: an assignment, a function call or an if.
/// </summary>
/// <param name="Line">1-based line, for error messages.</param>
/// <param name="Column">1-based column, for error messages.</param>
public abstract record Statement(int Line, int Column);
