namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CIsLetterCharacter
{
    /// <summary>
    ///     {Letter}  ::= [A-Za-z] [0xC0-0xD6] [0xD8-0xF6] [0xF8-0x2FF].
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if currentChar is {Letter}.</returns>
    public static bool IsLetterChar(this IGenericLexer lexer)
    {
        // A = 0x41  Z = 0x5A
        // a = 0x61  z = 0x7A
        return lexer.CurrentChar >= 'A' && lexer.CurrentChar <= 'Z'
            || lexer.CurrentChar >= 'a' && lexer.CurrentChar <= 'z'
            || lexer.CurrentChar >= '\xC0' && lexer.CurrentChar <= '\xD6'
            || lexer.CurrentChar >= '\xD8' && lexer.CurrentChar <= '\xF6'
            || lexer.CurrentChar >= '\xF8' && lexer.CurrentChar <= '\x2FF';
    }
}
