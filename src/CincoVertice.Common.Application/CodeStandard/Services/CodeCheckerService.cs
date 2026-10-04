using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Services;

/// <summary>
///     Checks files and folders for code standard compliance.
///     Each line is checked by <see cref="ILineCheckerService"/>.
/// </summary>
public class CodeCheckerService : ICodeCheckerService
{
    private static readonly string[] _skippedFolders = ["bin", "obj", ".git", ".vs"];

    private static readonly string[] _generatedFileSuffixes = [".Designer.cs", ".g.cs", ".g.i.cs"];

    private readonly ILineCheckerService _lineChecker;

    public CodeCheckerService(ILineCheckerService lineChecker)
    {
        _lineChecker = lineChecker;
    }

    public List<ErrorModel> CheckFile(string filePath)
    {
        List<ErrorModel> errors = [];

        if (!File.Exists(filePath))
        {
            errors.Add(Errors.New(0, nameof(Errors.CH0001), Errors.CH0001));

            return errors;
        }

        List<LineModel> lines = [];

        try
        {
            using StreamReader reader = new(filePath);

            lines = LineParser.GetLines(reader.ReadToEnd());
        }
        catch (Exception ex)
        {
            errors.Add(Errors.New(0, nameof(Errors.CH0002), Errors.CH0002.Replace("{message}", ex.Message)));

            return errors;
        }

        int previousIndentationLevel = 0;
        bool previousLineIsBlank = false;
        LineModel? previousCodeLine = null;

        foreach (var line in lines)
        {
            line.PreviousIndentationLevel = previousIndentationLevel;
            line.PreviousLineIsBlank = previousLineIsBlank;
            line.PreviousCodeLine = previousCodeLine;

            _lineChecker.CheckLine(line);

            // Blank lines never have their indentation computed, so they must not reset the
            // carried level; otherwise the next code line would falsely trip CH0014.
            if (!string.IsNullOrWhiteSpace(line.Content))
            {
                previousIndentationLevel = line.IndentationLevel;
            }

            previousLineIsBlank = string.IsNullOrWhiteSpace(line.Content);

            if (IsCodeLine(line))
            {
                previousCodeLine = line;
            }

            errors.AddRange(line.Errors);
        }

        return errors;
    }

    /// <exception cref="DirectoryNotFoundException">The folder does not exist.</exception>
    public List<FileCheckResultModel> CheckFolder(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
        }

        List<FileCheckResultModel> results = [];

        foreach (string filePath in EnumerateSourceFiles(folderPath).Order(StringComparer.OrdinalIgnoreCase))
        {
            List<ErrorModel> errors = CheckFile(filePath);

            results.Add(new FileCheckResultModel { FilePath = filePath, Errors = errors });
        }

        return results;
    }

    /// <summary>
    ///     C# source files in the folder and its subfolders, without walking into skipped folders.
    /// </summary>
    private static IEnumerable<string> EnumerateSourceFiles(string folderPath)
    {
        // The EnumerationOptions overload avoids the legacy pattern match where *.cs also matches *.csx
        EnumerationOptions options = new() { IgnoreInaccessible = true };
        Stack<string> folders = new([folderPath]);

        while (folders.Count > 0)
        {
            string folder = folders.Pop();

            foreach (string filePath in Directory.EnumerateFiles(folder, "*.cs", options))
            {
                if (!IsGeneratedFile(filePath))
                {
                    yield return filePath;
                }
            }

            foreach (string subfolder in Directory.EnumerateDirectories(folder, "*", options))
            {
                if (!_skippedFolders.Contains(Path.GetFileName(subfolder), StringComparer.OrdinalIgnoreCase))
                {
                    folders.Push(subfolder);
                }
            }
        }
    }

    /// <summary>
    ///     False for blank, comment and preprocessor lines. Runs after the check, so a trailing comment is already
    ///     removed from <see cref="LineModel.TrimmedContent"/>.
    /// </summary>
    private static bool IsCodeLine(LineModel line)
    {
        string code = line.TrimmedContent;

        return code.Length > 0 && code[0] is not ('/' or '*' or '#');
    }

    private static bool IsGeneratedFile(string filePath)
    {
        return _generatedFileSuffixes.Any(suffix => filePath.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
    }
}
