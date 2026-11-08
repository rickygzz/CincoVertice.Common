using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Checkers.CSharp;
using CincoVertice.Common.Application.CodeStandard.Checkers.JavaScript;
using CincoVertice.Common.Application.CodeStandard.Helper;
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
    private static readonly ILineChecker[] _csharpCheckers =
    [
        // First, so whitespace-only and comment lines are checked too
        new InvisibleCharacterChecker(),
        new BlankLineChecker(),
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
        new BracePlacementChecker(),
        new BraceSpaceChecker(),
        new ParenthesisSpaceChecker(),
        new MultipleSpaceChecker(),
        new ForbiddenEndTokenChecker(),
        new ForbiddenTokenChecker(),
    ];

    // The rules that work in any language, with indentation by brackets; the C# code rules are left out
    private static readonly ILineChecker[] _javaScriptCheckers =
    [
        new InvisibleCharacterChecker(),
        new BlankLineChecker(),
        new TrailingWhitespaceChecker(),
        new LineLengthChecker(),
        new BraceIndentationChecker(),
        new CommentSpaceChecker(JavaScriptCodeScanner.Instance),
        new MultipleSpaceChecker(JavaScriptCodeScanner.Instance),
    ];

    private readonly ILineChecker[] _checkers;

    /// <summary>
    ///     Checks C# lines.
    /// </summary>
    public LineCheckerService()
        : this(_csharpCheckers)
    {
    }

    /// <param name="checkers">The checkers, in the order they run.</param>
    public LineCheckerService(IEnumerable<ILineChecker> checkers)
    {
        _checkers = [.. checkers];
    }

    /// <summary>
    ///     Checks JavaScript lines. The caller sets <see cref="LineModel.ExpectedIndentation"/>.
    /// </summary>
    public static LineCheckerService CreateJavaScript() => new(_javaScriptCheckers);

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
