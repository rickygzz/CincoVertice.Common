namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipUntilCharsOrEof
{
    /// <summary>
    ///     NextChar() while CurrentChar != char in chars or '\0'.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <param name="chars">Characters to look for.</param>
    public static void SkipUntilCharsOrEOF(this IGenericLexer lexer, char[] chars)
    {
        bool found;

        do
        {
            if (lexer.CurrentChar != '\0')
            {
                found = false;

                foreach (char c in chars)
                {
                    if (lexer.CurrentChar == c)
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    lexer.NextChar();
                }
            }
            else
            {
                // EOF. Exit loop.
                found = true;
            }
        }
        while (!found);
    }
}
