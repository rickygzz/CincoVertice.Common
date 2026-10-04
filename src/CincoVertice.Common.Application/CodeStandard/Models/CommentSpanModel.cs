namespace CincoVertice.Common.Application.CodeStandard.Models;

/// <summary>
///     A comment in C# code, including its // or /* */ delimiters.
/// </summary>
/// <param name="Start">Index of the first slash.</param>
/// <param name="End">Index after the comment (exclusive): the line end for //, after */ for /* */.</param>
/// <param name="IsBlock">True for a /* */ comment, false for a // comment.</param>
public readonly record struct CommentSpanModel(int Start, int End, bool IsBlock);
