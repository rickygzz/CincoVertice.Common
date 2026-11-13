namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>
///     A part of a script that has a value.
/// </summary>
/// <param name="Line">1-based line, for error messages.</param>
/// <param name="Column">1-based column, for error messages.</param>
public abstract record Expression(int Line, int Column);
