namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipUntilCharOrEof
{
    /// <summary>
    ///     Skip characters until it finds c or {EOF}.
    ///     <para>NextChar() while CurrentChar != c or '\0'.</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <param name="c">Character to look for.</param>
    public static void SkipUntilCharOrEOF(this IGenericLexer lexer, char c)
    {
        while (lexer.CurrentChar != c && lexer.CurrentChar != '\0')
        {
            lexer.NextChar();
        }
    }
}
