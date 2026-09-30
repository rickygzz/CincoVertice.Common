using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using CincoVertice.Common.Utils.Lexer.Tokens;

namespace CincoVertice.Common.Utils.Config.Tokens;

public static class CGetValue
{
    /// <summary>
    /// Gets key from CharIndex.
    /// {Value}           ::= ({NumericValue} | {String}) {EOL}
    /// {NumericValue}    ::= ({Hex} | {Oct} | {Bin} | {Number}).
    /// {String}          ::= ({String-Quoted} | {String-Unquoted})
    /// {String-Quoted}   ::= [[] ['] ({Printable}|{EOL})* ["] []] {Spaces}*
    /// {String-Unquoted} ::= {Printable}*
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>GenericToken.
    ///     <para>If successful, Type = Key.</para>
    ///     <para>Otherwise, Type = Null and returns CharIndex to initial position.</para>
    /// </returns>
    public static GenericToken GetValue(this ConfigLexer lexer)
    {
        int startPos = lexer.CharIndex;
        GenericToken tokenValue = lexer.GetNumericValue();

        if (tokenValue.Type == GenericTokenType.Null || !lexer.SkipSpacesAndTabsToNextLineOrEof())
        {
            // Not a numeric value, must be a string.
            lexer.Char(startPos);

            tokenValue = lexer.GetString();
        }

        return tokenValue;
    }
}
