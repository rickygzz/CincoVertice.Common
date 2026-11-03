namespace CincoVertice.Common.Utils.Highlighting;

/// <summary>Finds what to color in one language's code.</summary>
public interface ICodeHighlighter
{
    /// <summary>
    ///     Finds the spans, in order and without overlaps. It never throws, since code being typed is usually broken.
    /// </summary>
    IReadOnlyList<CodeSpan> Find(string text);
}
