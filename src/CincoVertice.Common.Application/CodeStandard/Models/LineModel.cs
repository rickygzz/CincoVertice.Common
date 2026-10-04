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

    public int IndentationLevel { get; set; } = 0;

    public LineEndingEnum LineEnding = LineEndingEnum.None;

    public List<ErrorModel> Errors { get; set; } = [];

    public void AddError(string code, string message)
    {
        Errors.Add(new ErrorModel { Line = Number, Code = code, Message = message });
    }
}
