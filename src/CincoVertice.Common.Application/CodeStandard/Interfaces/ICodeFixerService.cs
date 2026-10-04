using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

public interface ICodeFixerService
{
    /// <summary>
    ///     Runs the fixers on a file and writes it back only when they changed it, with the same encoding
    ///     (including whether it has a BOM). A file that can't be read or written is skipped, not thrown.
    /// </summary>
    /// <exception cref="FileNotFoundException">The file does not exist.</exception>
    FileFixResultModel FixFile(string filePath);

    /// <summary>
    ///     Fixes every C# file in a folder and its subfolders; the same files as
    ///     <see cref="ICodeCheckerService.CheckFolder"/>.
    /// </summary>
    /// <returns>One result per file, sorted by path, including unchanged files.</returns>
    /// <exception cref="DirectoryNotFoundException">The folder does not exist.</exception>
    List<FileFixResultModel> FixFolder(string folderPath);
}
