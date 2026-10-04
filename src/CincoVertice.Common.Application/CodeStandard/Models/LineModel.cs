namespace CincoVertice.Common.Application.CodeStandard.Models;

public class LineModel
{
    public string Content { get; set; } = string.Empty;

    /// <summary>
    ///     <see cref="Content"/> without surrounding whitespace. After the comment checker runs, it also excludes
    ///     a trailing // comment, so the code rules only see code.
    /// </summary>
    public string TrimmedContent { get; set; } = string.Empty;

    public int Number { get; set; } = 0;

    public int PreviousIndentationLevel { get; set; } = 0;

    /// <summary>
    ///     True when the previous line is empty or whitespace only. Set by the caller, like
    ///     <see cref="PreviousIndentationLevel"/>.
    /// </summary>
    public bool PreviousLineIsBlank { get; set; } = false;

    /// <summary>
    ///     The closest previous line with code, skipping blank, comment and preprocessor lines. Set by the caller,
    ///     so checkers can follow statements that span lines.
    /// </summary>
    public LineModel? PreviousCodeLine { get; set; }

    /// <summary>
    ///     Set by the missing braces checker: parentheses of an if condition still open at the end of this line.
    /// </summary>
    public int OpenConditionParens { get; set; } = 0;

    /// <summary>
    ///     Set by the missing braces checker: this line ends an if or else header with no body, so the next code
    ///     line must start with {.
    /// </summary>
    public bool AwaitsOpeningBrace { get; set; } = false;

    public int IndentationLevel { get; set; } = 0;

    public LineEndingEnum LineEnding = LineEndingEnum.None;

    public List<ErrorModel> Errors { get; set; } = [];

    public void AddError(string code, string message)
    {
        Errors.Add(new ErrorModel { Line = Number, Code = code, Message = message });
    }
}
