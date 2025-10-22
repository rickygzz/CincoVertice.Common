using CincoVertice.Common.Ui.Rtf.Models;
using CincoVertice.Common.Utils.Helpers;
using System.Text;

namespace CincoVertice.Common.Ui.Win.Controls.Rtf.Core;

public class RtfText
{
    private readonly List<Font> _fonts;
    private readonly List<Color> _colors = [];

    private string _header = string.Empty;
    private string _content = string.Empty;
    private string _footer = string.Empty;

    private readonly List<RtfContent> _contentList = [];

    public RtfText(string defaultFount = "Courier New", float defaultFontSize = 11)
    {
        _fonts =
        [
            new Font(defaultFount, defaultFontSize)
        ];

        UpdateHeader();

        UpdateFooter();
    }

    public void UpdateDefaultFont(string defaultFont, float defaultFontSize)
    {
        _fonts[0] = new Font(defaultFont, defaultFontSize);
    }

    public string Rtf(bool updateContent = false)
    {
        if (updateContent)
        {
            UpdateContent();
        }

        StringBuilder sb = new StringBuilder(_header.Length + _content.Length + _footer.Length);

        sb.Append(_header);
        sb.Append(_content);
        sb.Append(_footer);

        return sb.ToString();
    }

    private void UpdateHeader()
    {
        StringBuilder sb = new StringBuilder(256 + _fonts.Count * 160 + _colors.Count * 25);

        sb.Append(@"{\rtf1\ansi\ansicpg1252\deff0");
        sb.Append(@"{\fonttbl{");

        for (int i = 0; i < _fonts.Count; i++)
        {
            sb.Append(@"\f");
            sb.Append(i);
            sb.Append(@"\fnil\fcharset0 ");
            sb.Append(_fonts[i].FontFamily.Name);
            sb.Append("; ");
        }

        sb.Append("}}");

        sb.Append(@"{\colortbl ;");
        foreach (var color in _colors)
        {
            sb.Append(@"\red");
            sb.Append(color.R.ToString());
            sb.Append(@"\green");
            sb.Append(color.G.ToString());
            sb.Append(@"\blue");
            sb.Append(color.B.ToString());
            sb.Append(';');
        }
        sb.Append('}');

        _header = sb.ToString();
    }

    private void UpdateContent()
    {
        if (_contentList.Count == 0)
        {
            _content = @"\viewkind4\uc1\pard\lang2058\f0\fs22";

            return;
        }

        var sb = new StringBuilder(EstimateContentCapacity());

        sb.Append(@"\viewkind4\uc1\pard\lang2058");
        if (_contentList[0].Format.FontIndex == -1)
        {
            sb.Append(@"\f0");
        }
        if (Equals(_contentList[0].Format.Size, -1))
        {
            sb.Append(@"\fs22\smult1\sl480");
        }

        RtfFormat prevFormat = new();

        for (int i = 0; i < _contentList.Count; i++)
        {
            sb.Append(_contentList[i].Rtf(prevFormat));

            prevFormat.Bold = _contentList[i].Format.Bold;
            prevFormat.Italic = _contentList[i].Format.Italic;
            prevFormat.ForeColorIndex = _contentList[i].Format.ForeColorIndex;
            prevFormat.HighlightColorIndex = _contentList[i].Format.HighlightColorIndex;
            prevFormat.Size = _contentList[i].Format.Size;
            prevFormat.FontIndex = _contentList[i].Format.FontIndex;
            prevFormat.LineHeight = _contentList[i].Format.LineHeight;
        }

        _content = sb.ToString();
    }

    private void UpdateFooter()
    {
        StringBuilder sb = new(20);

        sb.Append(@"\par}");

        _footer = sb.ToString();
    }

    private int EstimateContentCapacity()
    {
        if (_contentList.Count == 0)
        {
            return 0;
        }

        int contentLength = 42;
        int bold = -1;
        int italic = -1;
        int highlightColor = -1;
        int foreColor = -1;
        float size = -1;
        int font = -1;

        for (int i = 0; i < _contentList.Count; i++)
        {
            contentLength += _contentList[i].Text.Length;

            for (int o = 0; o < _contentList[i].Text.Length; o++)
            {
                if (_contentList[i].Text[o] == '\\' || _contentList[i].Text[o] == '{' || _contentList[i].Text[o] == '}')
                {
                    contentLength++;
                }
                else if (_contentList[i].Text[o] == 0x0A)
                {
                    o++;
                    if (o < _contentList[i].Text.Length && _contentList[i].Text[o] != 0x0C)
                    {
                        o--;
                    }

                    contentLength += 5;
                }
                else if (_contentList[i].Text[o] == 0x0C)
                {
                    o++;
                    if (o < _contentList[i].Text.Length && _contentList[i].Text[o] != 0x0A)
                    {
                        o--;
                    }

                    contentLength += 5;
                }
            }

            if (_contentList[i].Format.Bold != bold)
            {
                contentLength += 4;
                bold = _contentList[i].Format.Bold;
            }

            if (_contentList[i].Format.Italic != italic)
            {
                contentLength += 4;
                italic = _contentList[i].Format.Italic;
            }

            if (_contentList[i].Format.HighlightColorIndex != highlightColor)
            {
                contentLength += 18;
                highlightColor = _contentList[i].Format.HighlightColorIndex;
            }

            if (_contentList[i].Format.ForeColorIndex != foreColor)
            {
                contentLength += 11;
                foreColor = _contentList[i].Format.ForeColorIndex;
            }

            if (!FloatingPointHelper.AreEqual(_contentList[i].Format.Size, size))
            {
                contentLength += 11;
                size = _contentList[i].Format.Size;
            }

            if (_contentList[i].Format.FontIndex != font)
            {
                contentLength += 10;
                font = _contentList[i].Format.FontIndex;
            }
        }

        return contentLength;
    }

    public void AddColor(params Color[] colors)
    {
        foreach (var color in colors)
        {
            bool found = false;
            foreach (var existingColor in _colors)
            {
                if (existingColor.R == color.R
                    && existingColor.G == color.G
                    && existingColor.B == color.B)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                _colors.Add(color);
            }
        }

        UpdateHeader();
    }

    public void AddFont(params Font[] fonts)
    {
        foreach (var font in fonts)
        {
            bool found = false;

            foreach (var exitingFont in _fonts)
            {
                if (exitingFont.Equals(font))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                _fonts.Add(font);
            }
        }

        UpdateHeader();
    }

    public void AddText(string text, RtfFormat? format = null)
    {
        _contentList.Add(new RtfContent(text, format));
    }

    public void AddNewLine()
    {
        AddText("\n");
    }
}
