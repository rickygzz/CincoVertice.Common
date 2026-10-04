using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0020 and CH0021: /// and // must be followed by a space, in comment lines and in trailing comments
///     (code // comment). Empty comments and four or more slashes are allowed. Fixed by CommentSpaceFixer.
///     <para>A comment line stops parsing. A trailing comment is removed from <see cref="LineModel.TrimmedContent"/>,
///     so the following code rules only see the code, not the comment text.</para>
/// </summary>
public class CommentSpaceChecker : ILineChecker
{
    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        if (line.TrimmedContent.StartsWith("//"))
        {
            CheckSpaceAfterSlashes(line, line.TrimmedContent);

            return false;
        }

        // // inside a string, as in "http://", is not a comment
        int commentStart = CSharpLiteralScanner.FindLineComment(line.TrimmedContent);

        if (commentStart >= 0)
        {
            CheckSpaceAfterSlashes(line, line.TrimmedContent[commentStart..]);

            line.TrimmedContent = line.TrimmedContent[..commentStart].TrimEnd();
        }

        return true;
    }

    private static void CheckSpaceAfterSlashes(LineModel line, string comment)
    {
        int slashes = 0;

        while (slashes < comment.Length && comment[slashes] == '/')
        {
            slashes++;
        }

        if (slashes > 3 || slashes == comment.Length || comment[slashes] == ' ')
        {
            return;
        }

        if (slashes == 3)
        {
            line.AddError(nameof(Errors.CH0020), Errors.CH0020);
        }
        else
        {
            line.AddError(nameof(Errors.CH0021), Errors.CH0021);
        }
    }
}
