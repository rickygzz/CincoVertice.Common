using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Where comments and literals meet line boundaries, for line-based fixers that must not edit them.
///     FixerPipeline masks literals to one line, but fixers using this stay safe on their own.
/// </summary>
public static class LineSpans
{
    /// <summary>
    ///     For each line: whether it starts inside a /* */ comment or a multi-line literal, and whether it ends
    ///     inside one.
    /// </summary>
    public static (bool StartsInside, bool EndsInside)[] Find(string content, List<LineModel> lines)
    {
        var spans = CSharpLiteralScanner.FindComments(content)
            .Where(comment => comment.IsBlock)
            .Select(comment => (comment.Start, comment.End))
            .Concat(CSharpLiteralScanner.FindLiterals(content))
            .OrderBy(span => span.Start)
            .ToList();

        var result = new (bool StartsInside, bool EndsInside)[lines.Count];
        int next = 0;
        int lineStart = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            int lineEnd = lineStart + lines[i].Content.Length;

            while (next < spans.Count && spans[next].End <= lineStart)
            {
                next++;
            }

            bool startsInside = next < spans.Count && spans[next].Start < lineStart;
            bool endsInside = spans.Skip(next).TakeWhile(span => span.Start < lineEnd).Any(span => span.End > lineEnd);

            result[i] = (startsInside, endsInside);
            lineStart = lineEnd + lines[i].LineEnding.ToText().Length;
        }

        return result;
    }

    /// <summary>
    ///     Where the code of a line ends: at the start of a trailing comment, or at the line end.
    /// </summary>
    public static int CodeEnd(string lineContent)
    {
        var comments = CSharpLiteralScanner.FindComments(lineContent).ToList();

        if (comments.Count > 0 && comments[^1].End >= lineContent.TrimEnd().Length)
        {
            return comments[^1].Start;
        }

        return lineContent.Length;
    }
}
