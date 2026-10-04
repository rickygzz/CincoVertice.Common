using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0021: // must be followed by a space, in comment lines and in trailing comments (code // comment).
///     <para>A comment line stops parsing. A trailing comment is removed from <see cref="LineModel.TrimmedContent"/>,
///     so the following code rules only see the code, not the comment text.</para>
/// </summary>
public class CommentChecker : ILineChecker
{
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
        if (comment.Length > 2 && comment[2] != ' ' && comment[2] != '/')
        {
            line.AddError(nameof(Errors.CH0021), Errors.CH0021);
        }
    }
}
