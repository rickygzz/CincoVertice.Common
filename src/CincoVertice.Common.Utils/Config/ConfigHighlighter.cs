using System.Globalization;
using CincoVertice.Common.Utils.Highlighting;
using CincoVertice.Common.Utils.Lexer;

namespace CincoVertice.Common.Utils.Config;

/// <summary>
///     Finds what to color in a config file, from the tokens of <see cref="ConfigLexer"/>. A # after the value starts a
///     comment, unless it is a shortcut's own key: the lexer keeps it in the value, as in "Ctrl+# # note".
/// </summary>
public sealed class ConfigHighlighter : ICodeHighlighter
{
    public IReadOnlyList<CodeSpan> Find(string text)
    {
        List<CodeSpan> spans = [];
        ConfigLexer lexer = new(text);

        for (int i = 0; i < lexer.Tokens.Count; i++)
        {
            GenericToken token = lexer.Tokens.Token(i);
            int length = Math.Min(token.Length, text.Length - token.StartPos);

            if (token.StartPos < 0 || length <= 0)
            {
                continue;
            }

            if (token.Type == ConfigTokenType.Comment)
            {
                spans.Add(new CodeSpan(token.StartPos, length, CodeSpanKind.Comment));
            }
            else if (token.Type == ConfigTokenType.Key)
            {
                spans.Add(new CodeSpan(token.StartPos, length, CodeSpanKind.Key));
            }
            else if (token.Type == ConfigTokenType.StringQuoted)
            {
                spans.Add(new CodeSpan(token.StartPos, length, CodeSpanKind.Text));
            }
            else
            {
                AddValue(spans, text, token.StartPos, length);
            }
        }

        return spans;
    }

    private static void AddValue(List<CodeSpan> spans, string text, int start, int length)
    {
        string value = text.Substring(start, length);
        int comment = FindComment(value);
        string withoutComment = (comment < 0 ? value : value[..comment]).TrimEnd();

        if (withoutComment.Length > 0)
        {
            spans.Add(new CodeSpan(start, withoutComment.Length, KindOf(withoutComment)));
        }

        if (comment >= 0)
        {
            spans.Add(new CodeSpan(start + comment, length - comment, CodeSpanKind.Comment));
        }
    }

    private static int FindComment(string value)
    {
        for (int i = 0; i < value.Length; i++)
        {
            if (value[i] == '#'
                && (i == 0 || char.IsWhiteSpace(value[i - 1]))
                && !value[..i].TrimEnd().EndsWith('+'))
            {
                return i;
            }
        }

        return -1;
    }

    private static CodeSpanKind KindOf(string value)
    {
        if (bool.TryParse(value, out _))
        {
            return CodeSpanKind.Keyword;
        }

        return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out _)
            ? CodeSpanKind.Number
            : CodeSpanKind.Text;
    }
}
