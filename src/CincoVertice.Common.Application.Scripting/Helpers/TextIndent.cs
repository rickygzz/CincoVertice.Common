namespace CincoVertice.Common.Application.Scripting.Helpers;

/// <summary>
///     The indentation rule of multi-line texts: what is before the closing """ is removed from every line, so the
///     text can be indented with the script.
/// </summary>
internal static class TextIndent
{
    /// <param name="lines">The lines between the opening and the closing """.</param>
    /// <param name="indent">What is before the closing """, e.g. four spaces; compared character by character.</param>
    /// <param name="text">The lines without the indent, joined with \n; a blank line becomes empty.</param>
    /// <param name="badLine">The index in <paramref name="lines"/> of the first line that doesn't start with the
    ///     indent, or -1.</param>
    /// <returns>False when a line that is not blank doesn't start with the indent.</returns>
    public static bool TryRemove(IReadOnlyList<string> lines, string indent, out string text, out int badLine)
    {
        List<string> result = [];

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Trim().Length == 0)
            {
                result.Add(string.Empty);
            }
            else if (lines[i].StartsWith(indent, StringComparison.Ordinal))
            {
                result.Add(lines[i][indent.Length..]);
            }
            else
            {
                text = string.Empty;
                badLine = i;

                return false;
            }
        }

        text = string.Join('\n', result);
        badLine = -1;

        return true;
    }
}
