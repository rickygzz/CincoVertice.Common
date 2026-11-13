using System.Text;
using CincoVertice.Common.Application.CodeStandard.Fixers;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Services;

/// <summary>
///     Fixes files and folders for the code standard, in place. Each file's content goes through the
///     <see cref="FixerPipeline"/> of its language.
/// </summary>
public class CodeFixerService : ICodeFixerService
{
    // Without a BOM, a file must be valid UTF-8: decoding e.g. a Windows-1252 file as UTF-8 and writing it back
    // would replace its accented characters for good. With a BOM, the reader switches to the BOM's encoding.
    private static readonly UTF8Encoding _strictUtf8 =
        new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    private readonly IReadOnlyList<CodeLanguageModel> _languages;

    /// <summary>
    ///     Fixes C# files only, with the given pipeline.
    /// </summary>
    public CodeFixerService(FixerPipeline pipeline)
        : this([CodeLanguageModel.CSharp with { Fixers = pipeline }])
    {
    }

    /// <param name="languages">
    ///     The languages to fix, each in its own files. A file of none of them is fixed as the first.
    /// </param>
    public CodeFixerService(IEnumerable<CodeLanguageModel> languages)
    {
        _languages = [.. languages];
    }

    public FileFixResultModel FixFile(string filePath, FixOptionsModel? options = null)
    {
        options ??= FixOptionsModel.Default;

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found.", filePath);
        }

        var result = new FileFixResultModel { FilePath = filePath };

        try
        {
            string content;
            Encoding encoding;

            using (StreamReader reader = new(filePath, _strictUtf8, detectEncodingFromByteOrderMarks: true))
            {
                content = reader.ReadToEnd();

                // Known only after reading: the BOM's encoding, or the strict UTF-8 without a BOM
                encoding = reader.CurrentEncoding;
            }

            FixerPipeline fixers = LanguageOf(filePath).Fixers;
            string fixedContent = (options.Fixers is null ? fixers : fixers.Only(options.Fixers)).Run(content);

            // The whole text, literals too: a multi-line string's line endings are the file's
            fixedContent = new LineEndingFixer(options.LineEnding).Fix(fixedContent);
            Encoding writeAs = EncodingOf(options.Encoding) ?? encoding;

            if (fixedContent != content || !IsSame(writeAs, encoding))
            {
                File.WriteAllText(filePath, fixedContent, writeAs);
                result.IsChanged = true;
            }
        }
        catch (DecoderFallbackException)
        {
            result.SkippedReason = "Not valid UTF-8; add a BOM or convert the file to UTF-8.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            result.SkippedReason = ex.Message;
        }

        return result;
    }

    public List<FileFixResultModel> FixFolder(string folderPath, FixOptionsModel? options = null)
    {
        var extensions = _languages.SelectMany(language => language.Extensions);

        return [.. SourceFileEnumerator.GetSourceFiles(folderPath, extensions).Select(path => FixFile(path, options))];
    }

    // What to write a file as; null to keep its own
    private static Encoding? EncodingOf(FileEncodingEnum encoding) => encoding switch
    {
        FileEncodingEnum.Utf8 => new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
        FileEncodingEnum.Utf8Bom => new UTF8Encoding(encoderShouldEmitUTF8Identifier: true),
        _ => null,
    };

    // The same text in the file: the same code page, and a BOM in both or in neither
    private static bool IsSame(Encoding a, Encoding b) =>
        a.CodePage == b.CodePage && a.GetPreamble().Length == b.GetPreamble().Length;

    private CodeLanguageModel LanguageOf(string filePath) =>
        _languages.FirstOrDefault(language => language.Covers(filePath)) ?? _languages[0];
}
