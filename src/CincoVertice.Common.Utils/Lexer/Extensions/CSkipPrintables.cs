namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipPrintables
{
    /// <summary>
    ///     Skips {Printable} characters found from current CharIndex.
    ///     <para>{Printable} ::= [0x09 0x20-0x07E 0x80-0xFFFD].</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>
    ///     If {Printable} characters are found, return true.
    ///     Otherwise, return false and leave CharIndex in its original position.
    /// </returns>
    public static bool SkipPrintables(this IGenericLexer lexer)
    {
        int startIndex = lexer.CharIndex;

        while (lexer.IsPrintableChar())
        {
            lexer.NextChar();
        }

        return startIndex != lexer.CharIndex;
    }
}
