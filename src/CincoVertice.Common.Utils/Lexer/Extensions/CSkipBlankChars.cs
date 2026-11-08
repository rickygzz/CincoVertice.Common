namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipBlankChars
{
    /// <summary>
    ///     Skips spaces, carriage returns, line feeds, form feed or tabs.
    ///     <para>{BlankChars}  ::= [' ' 0x09 0x0D 0x0A 0x0C].</para>
    ///     <para>Space, tab, carriage return (CR), line feed (LF) and form feed (FF).</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    public static void SkipBlankChars(this IGenericLexer lexer)
    {
        while (lexer.CurrentChar == '\x20'
            || lexer.CurrentChar == '\x09'
            || lexer.CurrentChar == '\x0D'
            || lexer.CurrentChar == '\x0A'
            || lexer.CurrentChar == '\x0C')
        {
            lexer.NextChar();
        }
    }
}
