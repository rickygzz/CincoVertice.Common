namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipCharsUntilStringOrEof
{
    /// <summary>
    ///     kip all characters up to str is found or {EOF}.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <param name="str">String to skip.</param>
    /// <returns>true if str was found and skipped, otherwise false.</returns>
    public static bool SkipCharsUntilStringOrEOF(this IGenericLexer lexer, string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return true;
        }

        do
        {
            if (lexer.CurrentChar == str[0])
            {
                bool found = true;
                for (int i = 1; i < str.Length; i++)
                {
                    lexer.NextChar();

                    if (lexer.CurrentChar != str[i])
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    lexer.NextChar();
                    return true;
                }
            }
            else
            {
                lexer.NextChar();
            }
        }
        while (lexer.CurrentChar != 0);

        return false;
    }
}
