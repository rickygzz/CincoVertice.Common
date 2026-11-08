namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CIsBlankChar
{
    /// <summary>
    /// {BlankChar} ::= [' ' 0x09 0x0D 0x0A].
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if currentChar is {BlankChar}.</returns>
    public static bool IsBlankChar(this IGenericLexer lexer)
    {
        return lexer.CurrentChar == '\x09' || lexer.CurrentChar == '\x0A'
            || lexer.CurrentChar == '\x0D' || lexer.CurrentChar == '\x20';
    }
}
