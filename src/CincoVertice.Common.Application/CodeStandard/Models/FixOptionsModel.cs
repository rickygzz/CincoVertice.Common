namespace CincoVertice.Common.Application.CodeStandard.Models;

/// <summary>What a fix does to each file, besides what its language's fixers do.</summary>
/// <param name="Fixers">
///     The fixers to run, by <see cref="Services.FixerPipeline.NameOf"/>, in their usual order; all when null.
/// </param>
/// <param name="LineEnding">The line ending every line gets; <see cref="LineEndingEnum.None"/> keeps them.</param>
/// <param name="Encoding">How the file is written.</param>
public sealed record FixOptionsModel(
    IReadOnlyCollection<string>? Fixers = null,
    LineEndingEnum LineEnding = LineEndingEnum.None,
    FileEncodingEnum Encoding = FileEncodingEnum.Keep)
{
    /// <summary>Every fixer; line endings and encoding as they are.</summary>
    public static FixOptionsModel Default { get; } = new();

    /// <summary>Whether it would change anything at all.</summary>
    public bool DoesAnything =>
        Fixers is null || Fixers.Count > 0 || LineEnding != LineEndingEnum.None || Encoding != FileEncodingEnum.Keep;
}
