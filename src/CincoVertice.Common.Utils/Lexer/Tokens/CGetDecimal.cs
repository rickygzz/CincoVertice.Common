using CincoVertice.Common.Utils.Lexer.Extensions;

namespace CincoVertice.Common.Utils.Lexer.Tokens;

public static class CGetDecimal
{
    /// <summary>
    ///     Gets Decimal from CharIndex.
    ///     <para>Decimal  ::= ('-'? '.' {Digit}+) | ({Integer} '.'? {Digit}*).</para>
    ///     <para>Integer  ::= '-'? {Digit}+.</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>GenericToken.
    ///     <para>If successful, Type = Decimal.</para>
    ///     <para>Otherwise, Type = Null and returns CharIndex to initial position.</para>
    /// </returns>
    public static GenericToken GetDecimal(this IGenericLexer lexer)
    {
        GenericToken token = lexer.NullToken();

        lexer.SkipChar('-');

        int integerIndex = lexer.CharIndex;
        if (!lexer.SkipDigits())
        {
            integerIndex = -1;
        }

        if (lexer.SkipChar('.') || integerIndex != -1)
        {
            // A decimal char or integer value was found.
            if (!lexer.SkipDigits() && integerIndex == -1)
            {
                // No digits returned after . and has no integers before (cases '-.' or '.')
                // Return GenericTokenType.Null in original position
                lexer.Char(token.StartPos);
                return token;
            }

            token.Length = lexer.CharIndex - token.StartPos;
            token.Type = GenericTokenType.Decimal;
        }
        else
        {
            // Return GenericTokenType.Null in original position
            lexer.Char(token.StartPos);
        }

        return token;
    }
}
