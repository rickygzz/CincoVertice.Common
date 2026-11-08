namespace CincoVertice.Common.Ui.Win.Controls.Rtf;

/// <summary>
/// A problem shown with a squiggly line under the code. Line and Column start at 1. Without a Column it marks the
/// whole line, without its indentation: the code checker only reports lines.
/// </summary>
public sealed record CodeMark(int Line, string Message, int? Column = null, int? Length = null, Color? Color = null)
{
    /// <summary>
    ///     Where the mark falls inside its line's text, always inside it. A length of 0 means a blank spot.
    /// </summary>
    public (int Start, int Length) FindRange(string lineText)
    {
        if (Column is int column)
        {
            int start = Math.Clamp(column - 1, 0, lineText.Length);
            int available = lineText.Length - start;

            return (start, Math.Clamp(Length ?? available, 0, available));
        }

        string trimmed = lineText.TrimStart();

        return (lineText.Length - trimmed.Length, trimmed.TrimEnd().Length);
    }
}
