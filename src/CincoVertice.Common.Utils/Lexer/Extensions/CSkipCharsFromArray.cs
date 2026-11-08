namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipCharsFromArray
{
    /// <summary>
    ///     Skips characters from array c if found from current CharIndex.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <param name="c">Characters to skip.</param>
    /// <returns>
    ///     If at least one char in array is found, return true.
    ///     Otherwise, return false and leave CharIndex in its original position.
    /// </returns>
    public static bool SkipCharsFromArray(this IGenericLexer lexer, char[] c)
    {
        int startIndex = lexer.CharIndex;

        if (c.Contains(lexer.CurrentChar))
        {
            lexer.NextChar();
        }

        return startIndex != lexer.CharIndex;
    }
}
