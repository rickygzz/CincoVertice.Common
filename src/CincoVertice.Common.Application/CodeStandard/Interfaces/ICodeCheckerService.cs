using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

public interface ICodeCheckerService
{
    List<ErrorModel> CheckFile(string filePath);

    /// <summary>
    ///     Checks every C# and JavaScript file in a folder and its subfolders, each with its language's rules.
    ///     Build output (bin, obj), hidden tool folders (.git, .vs), installed packages (node_modules) and generated
    ///     files (*.Designer.cs, *.g.cs, *.min.js) are skipped.
    /// </summary>
    /// <param name="folderPath">The folder to check.</param>
    /// <returns>One result per checked file, sorted by path, including files without errors.</returns>
    List<FileCheckResultModel> CheckFolder(string folderPath);
}
