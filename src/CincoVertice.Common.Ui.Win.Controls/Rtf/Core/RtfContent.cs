using CincoVertice.Common.Ui.Rtf.Models;
using CincoVertice.Common.Utils.Helpers;
using System.Text;

namespace CincoVertice.Common.Ui.Win.Controls.Rtf.Core;

public class RtfContent
{
    public RtfFormat Format { get; private set; } = new();
    public string Text { get; private set; }

    public RtfContent(string text, RtfFormat? format = null)
    {
        Text = text;

        if (format == null)
        {
            // All properties null: keep the previous formatting
            return;
        }

        Format.Bold = format.Bold;
        Format.Italic = format.Italic;
        Format.ForeColorIndex = format.ForeColorIndex;
        Format.HighlightColorIndex = format.HighlightColorIndex;
        Format.Size = format.Size;
        Format.FontIndex = format.FontIndex;
        Format.LineHeight = format.LineHeight;
    }

    public string Rtf(RtfFormat prevFormat)
    {
        StringBuilder sb = new(Text.Length + 1024);

        // Write only the formatting that is set (not null) and differs from the previous text

        if (Format.Bold is bool bold && bold != prevFormat.Bold)
        {
            sb.Append(bold ? "\\b " : "\\b0 ");
        }

        if (Format.Italic is bool italic && italic != prevFormat.Italic)
        {
            sb.Append(italic ? "\\i " : "\\i0 ");
        }

        if (Format.HighlightColorIndex is int highlightColorIndex && highlightColorIndex != prevFormat.HighlightColorIndex)
        {
            sb.Append("\\highlight").Append(highlightColorIndex).Append(' ');
        }

        if (Format.ForeColorIndex is int foreColorIndex && foreColorIndex != prevFormat.ForeColorIndex)
        {
            sb.Append("\\cf").Append(foreColorIndex).Append(' ');
        }

        // RTF numeric parameters must be integers, and every control word must end with a space
        // (or another control word), otherwise text starting with a digit becomes part of the number.

        if (Format.Size is float size && !FloatingPointHelper.AreEqual(size, prevFormat.Size))
        {
            // \fs is in half-points
            sb.Append("\\fs").Append((int)Math.Round(size * 2)).Append(' ');
        }

        if (Format.LineHeight is double lineHeight && !FloatingPointHelper.AreEqual(lineHeight, prevFormat.LineHeight))
        {
            // With \slmult1, \sl is a multiple of single spacing in 240ths: 240 = single, 360 = 1.5 lines
            sb.Append("\\sl").Append((int)Math.Round(240 * lineHeight)).Append("\\slmult1 ");
        }

        if (Format.FontIndex is int fontIndex && fontIndex != prevFormat.FontIndex)
        {
            sb.Append("\\f").Append(fontIndex).Append(' ');
        }

        // Escape special chars
        for (int i = 0; i < Text.Length; i++)
        {
            char c = Text[i];

            if (c == '\\' || c == '{' || c == '}')
            {
                sb.Append('\\');
                sb.Append(c);
            }
            else if (c == 10)
            {
                i++;
                if (i < Text.Length && Text[i] != 13)
                {
                    i--;
                }

                sb.Append("\\par ");
            }
            else if (c == 13)
            {
                i++;
                if (i < Text.Length && Text[i] != 10)
                {
                    i--;
                }

                sb.Append("\\par ");
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
