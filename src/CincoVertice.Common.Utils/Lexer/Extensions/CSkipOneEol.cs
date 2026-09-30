namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipOneEol
{
    /// <summary>
    ///     {EOL}    ::= 0x0D 0x0A | 0x0A 0x0D | 0x0A | 0x0D | {EOF}.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if {EOL} was skipped or it reached {EOF}.</returns>
    public static bool SkipOneEOL(this IGenericLexer lexer)
    {
        if (lexer.CurrentChar == 0x0A)
        {
            lexer.NextChar();

            if (lexer.CurrentChar == 0x0D)
            {
                lexer.NextChar();
            }

            return true;
        }
        else if (lexer.CurrentChar == 0x0D)
        {
            lexer.NextChar();

            if (lexer.CurrentChar == 0x0A)
            {
                lexer.NextChar();
            }

            return true;
        }
        else if (lexer.CurrentChar == 0x00)
        {
            return true;
        }

        return false;
    }
}
