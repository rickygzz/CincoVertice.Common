namespace CincoVertice.Common.Application.CodeStandard.Models;

/// <summary>
///     Result of fixing one file for the code standard.
/// </summary>
public class FileFixResultModel
{
    public string FilePath { get; set; } = string.Empty;

    /// <summary>True when the fixers changed the content and the file was written.</summary>
    public bool IsChanged { get; set; } = false;

    /// <summary>
    ///     Why the file was left untouched (e.g. not valid UTF-8, or locked); null when it was processed.
    /// </summary>
    public string? SkippedReason { get; set; }
}
