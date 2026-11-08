namespace CincoVertice.Common.Utils.Strings;

public static class Delimiter
{
    /// <summary>
    ///     After. Gets the string after the delimiter.
    ///     <para>Example: After("A = B", "=", true) will return "B".</para>
    /// </summary>
    /// <param name="text">The string to work with.</param>
    /// <param name="delimiter">The string to look up within text, delimiting the text before and after.</param>
    /// <param name="trim">If true, it removes all leading and trailing white-spaces from the current string.</param>
    /// <returns>
    ///     If successful, gets the string after the delimiter. Otherwise, returns string.Empty if there
    ///     is no characters after delimiter or no delimiter is found.
    /// </returns>
    public static string After(string text, string delimiter, bool trim = false)
    {
        int textLength = text.Length;
        int delimiterLength = delimiter.Length;

        for (int i = 0; i < textLength - delimiterLength; i++)
        {
            if (text.Substring(i, delimiterLength) == delimiter)
            {
                if (trim)
                {
                    return text.Substring(i + delimiterLength).Trim();
                }

                return text.Substring(i + delimiterLength);
            }
        }

        return string.Empty;
    }
}
