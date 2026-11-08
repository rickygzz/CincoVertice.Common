namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipCharsToNextLineOrEof
{
    /// <summary>
    ///     Skips all characters, until it skips one {EOL} or {EOF}.
    ///     <para>{EOL}    ::= 0x0D 0x0A | 0x0A 0x0D | 0x0A | 0x0D | {EOF}.</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if {EOL} was skipped or it reached {EOF}.</returns>
    public static bool SkipCharsToNextLineOrEOF(this IGenericLexer lexer)
    {
        while (lexer.CurrentChar != 0xA && lexer.CurrentChar != 0xD && lexer.CurrentChar != 0)
        {
            lexer.NextChar();
        }

        return lexer.SkipOneEOL();
    }
}
