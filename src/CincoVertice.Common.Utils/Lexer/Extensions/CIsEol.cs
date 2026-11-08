namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CIsEol
{
    /// <summary>
    ///     {EOL}    ::= 0x0D 0x0A | 0x0A 0x0D | 0x0A | 0x0D | {EOF}.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if {EOL} or it reached {EOF}.</returns>
    public static bool IsEOL(this IGenericLexer lexer)
    {
        return lexer.CurrentChar == 0x0A || lexer.CurrentChar == 0x0D || lexer.CurrentChar == 0x00;
    }
}
