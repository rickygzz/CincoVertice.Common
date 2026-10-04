using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0030: a /// line must not contain an empty tag pair, such as &lt;summary&gt;&lt;/summary&gt; or
///     &lt;param name="x"&gt;&lt;/param&gt;. Fixed by EmptyXmlCommentFixer.
/// </summary>
public class EmptyXmlCommentChecker : ILineChecker
{
    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        if (XmlDocComment.IsDocLine(line.TrimmedContent) && XmlDocComment.ContainsEmptyTag(line.TrimmedContent))
        {
            line.AddError(nameof(Errors.CH0030), Errors.CH0030);

            return false;
        }

        return true;
    }
}
