namespace CincoVertice.Common.Utils.Config.Models;

/// <summary>
///     A line <see cref="ConfigLexer"/> could not read.
/// </summary>
/// <param name="Number">The line's number, from 1.</param>
/// <param name="Text">The line, without the spaces around it.</param>
public sealed record ConfigBadLine(int Number, string Text);
