using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     <see cref="CSharpLiteralScanner"/> as an <see cref="ICodeScanner"/>, for the rules that work in any language.
/// </summary>
public sealed class CSharpCodeScanner : ICodeScanner
{
    public static CSharpCodeScanner Instance { get; } = new();

    public IEnumerable<CommentSpanModel> FindComments(string content) => CSharpLiteralScanner.FindComments(content);

    public IEnumerable<(int Start, int End)> FindLiterals(string content) => CSharpLiteralScanner.FindLiterals(content);
}
