using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

public interface ICodeFixerService
{
    /// <summary>
    ///     Runs the fixers on a file, then gives it the line ending and encoding the options say, and writes it back
    ///     only when that changed it. With the default options it keeps its encoding (including whether it has a
    ///     BOM) and its line endings. A file that can't be read or written is skipped, not thrown.
    /// </summary>
    /// <param name="filePath">The file.</param>
    /// <param name="options">Which fixers, line ending and encoding; every fixer, keeping the rest, when null.</param>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    FileFixResultModel FixFile(string filePath, FixOptionsModel? options = null);

    /// <summary>
    ///     Fixes every C# and JavaScript file in a folder and its subfolders, as <see cref="FixFile"/> does; the same
    ///     files as <see cref="ICodeCheckerService.CheckFolder"/>.
    /// </summary>
    /// <param name="folderPath">The folder.</param>
    /// <param name="options">Which fixers, line ending and encoding; every fixer, keeping the rest, when null.</param>
    /// <returns>One result per file, sorted by path, including unchanged files.</returns>
    /// <exception cref="DirectoryNotFoundException">The folder does not exist.</exception>
    List<FileFixResultModel> FixFolder(string folderPath, FixOptionsModel? options = null);
}
