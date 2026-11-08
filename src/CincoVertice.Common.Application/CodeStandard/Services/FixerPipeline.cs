using CincoVertice.Common.Application.CodeStandard.Fixers;
using CincoVertice.Common.Application.CodeStandard.Fixers.CSharp;
using CincoVertice.Common.Application.CodeStandard.Fixers.JavaScript;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Services;

public sealed class FixerPipeline
{
    /// <summary>
    ///     Spaces per level in JavaScript, the same 4 as in C#: tabs and 2 spaces, also common there, are fixed.
    /// </summary>
    public const int JavaScriptIndentSize = 4;

    private readonly IReadOnlyList<IStringFixer> _fixers;
    private readonly ICodeScanner _scanner;

    /// <param name="fixers">The fixers, in the order they run.</param>
    /// <param name="scanner">Finds the literals to mask; C# when not given.</param>
    public FixerPipeline(IEnumerable<IStringFixer> fixers, ICodeScanner? scanner = null)
    {
        _fixers = [.. fixers];
        _scanner = scanner ?? CSharpCodeScanner.Instance;
    }

    /// <summary>
    ///     Runs every fixer in order, feeding each one the previous fixer's output.
    ///     String and char literals are masked first, so no fixer can change text inside them.
    ///     <para>Masked, any language reads like C# to the fixers: what's left is code, comments and "..."
    ///     placeholders. So the fixers that work in any language can use the C# scanner.</para>
    /// </summary>
    public string Run(string content)
    {
        var mask = new LiteralMask(_scanner);
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
            new BracePlacementFixer(),
            new MissingBracesFixer(),
            new ForbiddenStartTokenFixer(),
            new BlankLineFixer(),
            new IndentationFixer(),
            new BraceSpaceFixer(),
            new ParenthesisSpaceFixer(),
            new MultipleSpaceFixer(),
            new KeywordParenthesisSpaceFixer(),
            new TrailingOperatorFixer(),
            new TrailingWhitespaceFixer(),
            new TrailingNewLineFixer()
        ]);
    }

    /// <summary>
    ///     Builds the JavaScript formatting pipeline: the fixers that work in any language, in the same order as
    ///     <see cref="CreateFormatting"/>, with indentation by brackets instead of the C# one. Braces stay where
    ///     they are: on the line of their statement is the JavaScript way, and moving the { after a return to its
    ///     own line would change what the code does.
    /// </summary>
    public static FixerPipeline CreateJavaScriptFormatting()
    {
        return new FixerPipeline(
        [
            new InvisibleCharacterFixer(),
            new TypographicCharacterFixer(),
            new CommentSpaceFixer(),
            new BlankLineFixer(),
            new BraceIndentationFixer(JavaScriptIndentSize, JavaScriptCodeScanner.Instance),
            new MultipleSpaceFixer(),
            new TrailingWhitespaceFixer(),
            new TrailingNewLineFixer()
        ],
        JavaScriptCodeScanner.Instance);
    }
}
