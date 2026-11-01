namespace CincoVertice.Common.Application.Scripting.Models;

/// <param name="Type">Kind of token.</param>
/// <param name="Value">The token's text; for a text literal, its content without quotes and escapes.</param>
/// <param name="Line">1-based line, for error messages.</param>
/// <param name="Column">1-based column, for error messages.</param>
public record Token(TokenType Type, string Value, int Line, int Column);
