using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers.JavaScript;

/// <summary>
///     Fixes CH0036 (see BraceIndentationChecker): indents every line by the brackets around it, as
///     <see cref="BraceIndenter"/> works it out, replacing tabs and whatever indentation it had.
/// </summary>
public sealed class BraceIndentationFixer : IStringFixer
{
    private readonly BraceIndenter _indenter;

    /// <param name="indentSize">Spaces per level.</param>
    /// <param name="scanner">Finds the language's comments and literals.</param>
    public BraceIndentationFixer(int indentSize, ICodeScanner scanner)
    {
        _indenter = new BraceIndenter(indentSize, scanner);
    }

    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var lines = LineParser.GetLines(content);
        int?[] indents = _indenter.ExpectedIndents(content);
        var sb = new StringBuilder(content.Length);

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i].Content;

            if (indents[i] is { } spaces)
            {
                sb.Append(' ', spaces).Append(line.TrimStart(' ', '\t'));
            }
            else
            {
                sb.Append(line);
            }

            sb.Append(lines[i].LineEnding.ToText());
        }

        return sb.ToString();
    }
}
