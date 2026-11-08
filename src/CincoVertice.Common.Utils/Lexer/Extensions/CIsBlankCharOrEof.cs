namespace CincoVertice.Common.Utils.Lexer.Extensions;

/// <summary>
///     Friend/extension classes.
/// </summary>
public static class CIsBlankCharOrEof
{
    /// <summary>
    ///     {BlankCharOrEOF} ::= {BlankChar} + [00].
    ///     <para>{BlankChar} ::= [' ' 0x09 0x0D 0x0A].</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if currentChar is {Space}.</returns>
    public static bool IsBlankCharOrEOF(this IGenericLexer lexer)
    {
        return lexer.IsBlankChar() || lexer.CurrentChar == '\x00';
    }
}
