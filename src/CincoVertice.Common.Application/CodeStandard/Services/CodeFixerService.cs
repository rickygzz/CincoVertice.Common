using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Services;

/// <summary>
///     Fixes files and folders for the code standard, in place. Each file's content goes through
///     <see cref="FixerPipeline"/>.
/// </summary>
public class CodeFixerService : ICodeFixerService
{
    // Without a BOM, a file must be valid UTF-8: decoding e.g. a Windows-1252 file as UTF-8 and writing it back
    // would replace its accented characters for good. With a BOM, the reader switches to the BOM's encoding.
    private static readonly UTF8Encoding _strictUtf8 =
        new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    private readonly FixerPipeline _pipeline;

    public CodeFixerService(FixerPipeline pipeline)
    {
        _pipeline = pipeline;
    }

    public FileFixResultModel FixFile(string filePath)
    {
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

            string fixedContent = _pipeline.Run(content);

            if (fixedContent != content)
            {
                File.WriteAllText(filePath, fixedContent, encoding);
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

    public List<FileFixResultModel> FixFolder(string folderPath)
    {
        return [.. SourceFileEnumerator.GetSourceFiles(folderPath).Select(FixFile)];
    }
}
