namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

public interface IStringFixer
{
    /// <summary>
    ///     Applies a single code-standard fix to the given content and returns the result.
    /// </summary>
    /// <param name="content">The file content to fix.</param>
    /// <returns>The fixed content.</returns>
    string Fix(string content);
}
