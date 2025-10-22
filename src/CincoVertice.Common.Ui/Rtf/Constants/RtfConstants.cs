using CincoVertice.Common.Ui.Rtf.Models;

namespace CincoVertice.Common.Ui.Rtf.Constants;

public static class RtfConstants
{
    public static readonly RtfFormat Normal = new()
    {
        Bold = 0,
        Italic = 0,
        ForeColorIndex = 1,
        HighlightColorIndex = 0,
        Size = 11,
        FontIndex = 0,
        LineHeight = 1
    };

    public static readonly RtfFormat Bold = new() { Bold = 1 };
    public static readonly RtfFormat Regular = new() { Bold = 0 };

    public static readonly RtfFormat Highlight = new()
    {
        Bold = 0,
        Italic = 0,
        ForeColorIndex = 0,
        HighlightColorIndex = 1,
        Size = 11,
        FontIndex = 0,
        LineHeight = 1
    };
}
