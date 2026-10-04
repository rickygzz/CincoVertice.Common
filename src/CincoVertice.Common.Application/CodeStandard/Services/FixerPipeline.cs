using CincoVertice.Common.Application.CodeStandard.Fixers;
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
    /// </summary>
    public string Run(string content)
    {
        foreach (var fixer in _fixers)
        {
            content = fixer.Fix(content);
        }

        return content;
    }

    /// <summary>
    ///     Builds the default formatting pipeline. Order matters: the structural fixer runs first,
    ///     line-splitting fixers next, and trailing-whitespace cleanup last.
    /// </summary>
    public static FixerPipeline CreateFormatting()
    {
        return new FixerPipeline(
        [
            new FileScopedNamespaceFixer(),
            new IndentationFixer(),
            new KeywordParenthesisSpaceFixer(),
            new TrailingOperatorFixer(),
            new TrailingWhitespaceFixer()
        ]);
    }
}
