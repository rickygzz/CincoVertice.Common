using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Services;

namespace CincoVertice.Common.Application.CodeStandard.Models;

/// <summary>
///     The code standard of one language: which files it covers and how they are checked and fixed.
/// </summary>
/// <param name="Name">Shown to the user, e.g. "C#".</param>
/// <param name="Extensions">The extensions of its files, with the dot: ".cs".</param>
/// <param name="Scanner">Finds its comments and literals.</param>
/// <param name="LineChecker">Checks its lines.</param>
/// <param name="Fixers">Fixes its files.</param>
/// <param name="Indenter">
///     For a language indented by its brackets: works out <see cref="LineModel.ExpectedIndentation"/> for the
///     checker.
/// </param>
public sealed record CodeLanguageModel(
    string Name,
    IReadOnlyList<string> Extensions,
    ICodeScanner Scanner,
    ILineCheckerService LineChecker,
    FixerPipeline Fixers,
    BraceIndenter? Indenter = null)
{
    public static CodeLanguageModel CSharp { get; } = new(
        "C#",
        [".cs"],
        CSharpCodeScanner.Instance,
        new LineCheckerService(),
        FixerPipeline.CreateFormatting());

    public static CodeLanguageModel JavaScript { get; } = new(
        "JavaScript",
        [".js", ".mjs", ".cjs"],
        JavaScriptCodeScanner.Instance,
        LineCheckerService.CreateJavaScript(),
        FixerPipeline.CreateJavaScriptFormatting(),
        new BraceIndenter(FixerPipeline.JavaScriptIndentSize, JavaScriptCodeScanner.Instance));

    /// <summary>
    ///     Every language with a code standard.
    /// </summary>
    public static IReadOnlyList<CodeLanguageModel> All { get; } = [CSharp, JavaScript];

    public bool Covers(string filePath) =>
        Extensions.Contains(Path.GetExtension(filePath), StringComparer.OrdinalIgnoreCase);
}
