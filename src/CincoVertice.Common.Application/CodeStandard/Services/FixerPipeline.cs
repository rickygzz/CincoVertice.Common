using CincoVertice.Common.Application.CodeStandard.Fixers;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Services;

public sealed class FixerPipeline
{
    private readonly IReadOnlyList<IStringFixer> _fixers;

    public FixerPipeline(IEnumerable<IStringFixer> fixers)
    {
        _fixers = [.. fixers];
    }

    /// <summary>
    ///     Runs every fixer in order, feeding each one the previous fixer's output.
    ///     String and char literals are masked first, so no fixer can change text inside them.
    /// </summary>
    public string Run(string content)
    {
        var mask = new LiteralMask();
        string masked = mask.Mask(content);

        foreach (var fixer in _fixers)
        {
            masked = fixer.Fix(masked);
        }

        return mask.Restore(masked);
    }

    /// <summary>
    ///     Builds the default formatting pipeline. Order matters:
    ///     <list type="number">
    ///         <item>Character cleanup, so the other fixers see normal spaces and ASCII punctuation.</item>
    ///         <item>Comment fixers.</item>
    ///         <item>Structural fixers (namespace, braces), which add or remove lines and change nesting.</item>
    ///         <item>Blank lines, once no more lines are added or removed.</item>
    ///         <item>Indentation, after the structure is final.</item>
    ///         <item>Line-level fixers, then trailing whitespace and the final new line last.</item>
    ///     </list>
    ///     LineEndingFixer is not included: it converts to LF, and the codebase has both CRLF and LF files.
    /// </summary>
    public static FixerPipeline CreateFormatting()
    {
        return new FixerPipeline(
        [
            new InvisibleCharacterFixer(),
            new TypographicCharacterFixer(),
            new CommentSpaceFixer(),
            new EmptyXmlCommentFixer(),
            new FileScopedNamespaceFixer(),
            new MissingBracesFixer(),
            new BlankLineFixer(),
            new IndentationFixer(),
            new KeywordParenthesisSpaceFixer(),
            new TrailingOperatorFixer(),
            new TrailingWhitespaceFixer(),
            new TrailingNewLineFixer()
        ]);
    }
}
