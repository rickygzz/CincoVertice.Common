namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CEqualsString
{
    /// <summary>
    ///     Checks if str is found from current CharIndex. Leaves CharIndex in the same position.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <param name="str">String to check its existence.</param>
    /// <returns>
    ///     If str is found, returns true. Otherwise, returns false.
    /// </returns>
    public static bool EqualsString(this IGenericLexer lexer, string str)
    {
        int startIndex = lexer.CharIndex;

        if (lexer.SkipGivenString(str))
        {
            lexer.Char(startIndex);

            return true;
        }

        return false;
    }
}
