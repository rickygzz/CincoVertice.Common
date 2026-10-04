using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Services;

/// <summary>
///     Checks a line by running each <see cref="ILineChecker"/> in order. A checker returning false stops the
///     rest, e.g. comment lines skip the code rules.
/// </summary>
public class LineCheckerService : ILineCheckerService
{
    // Order matters. Checkers are stateless, so one instance of each is shared by all lines.
    private static readonly ILineChecker[] _checkers =
    [
        // First, so whitespace-only and comment lines are checked too
        new InvisibleCharacterChecker(),
        new BlankLineChecker(),
        new WhitespaceOnlyLineChecker(),
        new TrailingWhitespaceChecker(),
        new LineLengthChecker(),
        new IndentationChecker(),

        // Documentation and comment lines stop here
        new EmptyXmlCommentChecker(),
        new CommentSpaceChecker(),

        // Code rules
        new MissingBracesChecker(),
        new FileScopedNamespaceChecker(),
        new ForbiddenStartTokenChecker(),
        new KeywordParenthesisSpaceChecker(),
        new ForbiddenEndTokenChecker(),
        new ForbiddenTokenChecker(),
    ];

    /// <summary>
    ///     Checks line for code standard compliance.
    /// </summary>
    /// <param name="line">The line information.</param>
    public void CheckLine(LineModel line)
    {
        if (line.Content is null)
        {
            return;
        }

        line.TrimmedContent = line.Content.Trim();

        foreach (var checker in _checkers)
        {
            if (!checker.Check(line))
            {
                return;
            }
        }
    }
}
