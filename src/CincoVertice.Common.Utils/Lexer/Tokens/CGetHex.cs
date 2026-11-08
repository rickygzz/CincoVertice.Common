using CincoVertice.Common.Utils.Lexer.Extensions;

namespace CincoVertice.Common.Utils.Lexer.Tokens;

public static class CGetHex
{
    /// <summary>
    ///     Gets Hex from CharIndex.
    ///     <para>{HexDigit}   ::= {Digit} + [abcdefABCDEF].</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>GenericToken.
    ///     <para>If successful, Type = Hex.</para>
    ///     <para>Otherwise, Type = Null and returns CharIndex to initial position.</para>
    /// </returns>
    public static GenericToken GetHex(this IGenericLexer lexer)
    {
        GenericToken token = lexer.NullToken();

        if (lexer.SkipHexDigits())
        {
            token.Length = lexer.CharIndex - token.StartPos;
            token.Type = GenericTokenType.Hex;
        }

        return token;
    }
}
