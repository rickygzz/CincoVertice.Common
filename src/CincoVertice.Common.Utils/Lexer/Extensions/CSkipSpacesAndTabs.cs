namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipSpacesAndTabs
{
    /// <summary>
    ///     Skips spaces and tabs.
    ///     <para>{Space}  ::= [' ' 0x09].</para>
    ///     <para>Space, tab.</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    public static void SkipSpacesAndTabs(this IGenericLexer lexer)
    {
        while (lexer.CurrentChar == '\x20' || lexer.CurrentChar == '\x09')
        {
            lexer.NextChar();
        }
    }
}
