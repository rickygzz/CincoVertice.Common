using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

public interface ICodeCheckerService
{
    List<ErrorModel> CheckFile(string filePath);

    /// <summary>
    ///     Checks every C# file in a folder and its subfolders. Build output (bin, obj), hidden tool folders
    ///     (.git, .vs) and generated files (*.Designer.cs, *.g.cs) are skipped.
    /// </summary>
    /// <param name="folderPath">The folder to check.</param>
    /// <returns>One result per checked file, sorted by path, including files without errors.</returns>
    List<FileCheckResultModel> CheckFolder(string folderPath);
}
