namespace CincoVertice.Common.Application.CodeStandard.Models;

/// <summary>
///     Result of checking one file against the code standard.
/// </summary>
public class FileCheckResultModel
{
    public string FilePath { get; set; } = string.Empty;

    public List<ErrorModel> Errors { get; set; } = [];
}
