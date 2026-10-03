using CincoVertice.Common.Ui.Rtf.Models;

namespace CincoVertice.Common.Ui.Rtf.Constants;

public static class RtfConstants
{
    public static readonly RtfFormat Normal = new()
    {
        Bold = false,
        Italic = false,
        ForeColorIndex = 1,
        HighlightColorIndex = 0,
        Size = 11,
        FontIndex = 0,
        LineHeight = 1
    };

    public static readonly RtfFormat Console = new()
    {
        Bold = false,
        Italic = false,
        ForeColorIndex = 1,
        HighlightColorIndex = 0,
        Size = 12,
        LineHeight = 1.5
    };

    public static readonly RtfFormat Bold = new() { Bold = true };
    public static readonly RtfFormat Regular = new() { Bold = false };

    public static readonly RtfFormat Highlight = new()
    {
        Bold = false,
        Italic = false,
        ForeColorIndex = 0,
        HighlightColorIndex = 1,
        Size = 11,
        FontIndex = 0,
        LineHeight = 1
    };
}
