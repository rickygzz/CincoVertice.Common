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

            string content = reader.ReadToEnd();

            lines = LineParser.GetLines(content);
            MarkLinesInsideLiterals(content, lines);
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
        List<FileCheckResultModel> results = [];

        foreach (string filePath in SourceFileEnumerator.GetSourceFiles(folderPath))
        {
            List<ErrorModel> errors = CheckFile(filePath);

            results.Add(new FileCheckResultModel { FilePath = filePath, Errors = errors });
        }

        return results;
    }

    /// <summary>
    ///     False for blank, comment and preprocessor lines. Runs after the check, so a trailing comment is already
    ///     removed from <see cref="LineModel.TrimmedContent"/>.
    /// </summary>
    /// <summary>
    ///     Sets <see cref="LineModel.StartsInsideLiteral"/> on the lines that continue a multi-line verbatim or raw
    ///     string, which a single line cannot tell.
    /// </summary>
    private static void MarkLinesInsideLiterals(string content, List<LineModel> lines)
    {
        var literals = CSharpLiteralScanner.FindLiterals(content).ToList();
        int next = 0;
        int lineStart = 0;

        foreach (var line in lines)
        {
            while (next < literals.Count && literals[next].End <= lineStart)
            {
                next++;
            }

            line.StartsInsideLiteral = next < literals.Count && literals[next].Start < lineStart;
            lineStart += line.Content.Length + line.LineEnding.ToText().Length;
        }
    }

    private static bool IsCodeLine(LineModel line)
    {
        string code = line.TrimmedContent;

        return code.Length > 0 && code[0] is not ('/' or '*' or '#');
    }
}
