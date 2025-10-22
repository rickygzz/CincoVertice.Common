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
        if (format == null)
        {
            Format.Bold = -1;
            Format.Italic = -1;
            Format.ForeColorIndex = -1;
            Format.HighlightColorIndex = -1;
            Format.Size = -1;
            Format.FontIndex = -1;
            Format.LineHeight = -1;

            Text = text;

            return;
        }

        Format.Bold = format.Bold;
        Format.Italic = format.Italic;
        Format.ForeColorIndex = format.ForeColorIndex;
        Format.HighlightColorIndex = format.HighlightColorIndex;
        Format.Size = format.Size;
        Format.FontIndex = format.FontIndex;
        Format.LineHeight = format.LineHeight;

        Text = text;
    }

    public string Rtf(RtfFormat prevFormat)
    {
        StringBuilder sb = new(Text.Length + 1024);

        if (Format.Bold >= 0 && Format.Bold != prevFormat.Bold)
        {
            sb.Append(Format.Bold == 0 ? "\\b0 " : "\\b ");
        }

        if (Format.Italic >= 0 && Format.Italic != prevFormat.Italic)
        {
            sb.Append(Format.Italic == 0 ? "\\i0 " : "\\i ");
        }

        if (Format.HighlightColorIndex >= 0 && Format.HighlightColorIndex != prevFormat.HighlightColorIndex)
        {
            sb.Append("\\highlight").Append(Format.HighlightColorIndex).Append(' ');
        }

        if (Format.ForeColorIndex >= 0 && Format.ForeColorIndex != prevFormat.ForeColorIndex)
        {
            sb.Append("\\cf").Append(Format.ForeColorIndex).Append(' ');
        }

        if (Format.Size >= 0 && !FloatingPointHelper.AreEqual(Format.Size, prevFormat.Size))
        {
            sb.Append("\\fs").Append(Format.Size * 2).Append(' ');
        }

        if (Format.LineHeight >= 0 && !FloatingPointHelper.AreEqual(Format.LineHeight, prevFormat.LineHeight))
        {
            sb.Append(@"\smult1\sl" + 248 * Format.LineHeight);
        }

        if (Format.FontIndex >= 0 && Format.FontIndex != prevFormat.FontIndex)
        {
            sb.Append("\\f").Append(Format.FontIndex).Append(' ');
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
