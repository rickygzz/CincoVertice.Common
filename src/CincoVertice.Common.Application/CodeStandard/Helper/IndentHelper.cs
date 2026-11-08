namespace CincoVertice.Common.Application.CodeStandard.Helper;

public static class IndentHelper
{
    /// <summary>
    ///     Returns the leading whitespace (spaces and tabs) of a line.
    /// </summary>
    public static string GetIndent(string content)
    {
        int i = 0;
        while (i < content.Length && (content[i] == ' ' || content[i] == '\t'))
        {
            i++;
        }

        return content[..i];
    }
}
