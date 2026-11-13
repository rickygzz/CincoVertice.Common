using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

/// <summary>
///     Tells a language's comments and literals apart from its code, so rules don't change text inside literals or
///     read a // inside a string as a comment.
/// </summary>
public interface ICodeScanner
{
    /// <summary>
    ///     The // and /* */ comments, in order, ignoring comment delimiters inside literals. An unclosed /* comment
    ///     runs to the end of the content.
    /// </summary>
    /// <param name="content">Code: a whole file or a single line.</param>
    IEnumerable<CommentSpanModel> FindComments(string content);

    /// <summary>
    ///     The literals (strings, and whatever else the language has, like regular expressions), in order, with their
    ///     delimiters. For a whole file, literals spanning several lines are followed.
    /// </summary>
    /// <param name="content">Code: a whole file or a single line.</param>
    /// <returns>Start and end (exclusive) of each literal.</returns>
    IEnumerable<(int Start, int End)> FindLiterals(string content);
}
