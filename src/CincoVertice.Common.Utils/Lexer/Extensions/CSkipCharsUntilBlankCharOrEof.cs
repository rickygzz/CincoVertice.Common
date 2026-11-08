namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipCharsUntilBlankCharOrEof
{
    /// <summary>
    ///     NextChar() while CurrentChar != {BlankCharEOF}.
    ///     <para>{BlankCharEOF} ::= [' ' 0x09 0x0D 0x0A 0x00].</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    public static void SkipUntilBlankCharOrEOF(this IGenericLexer lexer)
    {
        while (!lexer.IsBlankCharOrEOF())
        {
            lexer.NextChar();
        }
    }
}
