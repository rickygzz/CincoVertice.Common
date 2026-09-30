using CincoVertice.Common.Utils.Lexer.Extensions;

namespace CincoVertice.Common.Utils.Lexer.Tokens;

public static class CGetNumericValue
{
    /// <summary>
    ///     Gets {NumericValue} from CharIndex.
    ///     <para>{NumericValue} ::= ({Hex} | {Oct} | {Bin} | {Number}).</para>
    ///     <para>{Hex} ::= '0x' {HexDigit}+.</para>
    ///     <para>{Bin} ::= '0b' {BinDigit}+.</para>
    ///     <para>{Oct} ::= '0o' {OctDigit}+.</para>
    ///     <para>{Number}     ::= {Integer} | {Decimal}.</para>
    ///     <para>{Integer}    ::= '-'? {Digit}+.</para>
    ///     <para>{Decimal}    ::= ('-'? '.' {Digit}+) | ({Integer} '.'? {Digit}*).</para>
    ///     <para>{Digit}      ::= [0123456789].</para>
    ///     <para>{HexDigit}   ::= {Digit} + [abcdefABCDEF].</para>
    ///     <para>{BinDigit}   ::= [01].</para>
    ///     <para>{OctDigit}   ::= [01234567].</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>GenericToken.
    ///     <para>If successful, Type = (Hex | Binary | Octal | Integer | Decimal).</para>
    ///     <para>Otherwise, Type = Null and returns CharIndex to initial position.</para>
    /// </returns>
    public static GenericToken GetNumericValue(this IGenericLexer lexer)
    {

        GenericToken token = lexer.NullToken();

        if (lexer.SkipChar('0'))
        {
            // Can be <Hex> | <Bin> | <Oct> | <Number>
            if (lexer.SkipChar('x') && lexer.SkipHexDigits())
            {
                // Alredy got '0x'. Get {HexDigit}+
                token.Length = lexer.CharIndex - token.StartPos;
                token.Type = GenericTokenType.Hex;

                return token;
            }
            else if (lexer.SkipChar('b') && lexer.SkipBinDigits())
            {
                // Already got '0b'. Get {BinDigit}+
                token.Length = lexer.CharIndex - token.StartPos;
                token.Type = GenericTokenType.Binary;

                return token;
            }
            else if (lexer.SkipChar('o') && lexer.SkipOctDigits())
            {
                // Already got '0o'. Get {OctDigit}+
                token.Length = lexer.CharIndex - token.StartPos;
                token.Type = GenericTokenType.Octal;

                return token;
            }

            // 0 was followed by something else (not x, not b, not o). It may be a <Number>
            lexer.Char(token.StartPos);
        }

        GenericToken integerToken = lexer.GetInteger();
        if (integerToken.Type == GenericTokenType.Integer)
        {
            if (lexer.SkipChar('.'))
            {
                // Can still be a decimal
                lexer.SkipDigits();

                return new GenericToken(
                    integerToken.StartPos,
                    lexer.CharIndex - integerToken.StartPos,
                    GenericTokenType.Decimal);
            }

            return integerToken;
        }

        GenericToken getDecimalToken = lexer.GetDecimal();
        if (getDecimalToken.Type == GenericTokenType.Decimal)
        {
            return getDecimalToken;
        }

        // Does not contain {Hex} | {Oct} | {Bin} | {Number}. Return null token.
        lexer.Char(token.StartPos);
        return token;
    }
}
