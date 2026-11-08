using CincoVertice.Common.Ui.Rtf.Models;

namespace CincoVertice.Common.Ui.Rtf.Constants;

public static class RtfConstants
{
    // RTF color table indexes. 0 is the automatic color; themes list Background first and Foreground second.
    public const int AutomaticColorIndex = 0;
    public const int BackgroundColorIndex = 1;
    public const int ForegroundColorIndex = 2;

    public static readonly RtfFormat Normal = new()
    {
        Bold = false,
        Italic = false,
        ForeColorIndex = ForegroundColorIndex,
        HighlightColorIndex = AutomaticColorIndex,
        Size = 11,
        FontIndex = 0,
        LineHeight = 1
    };

    public static readonly RtfFormat Console = new()
    {
        Bold = false,
        Italic = false,
        ForeColorIndex = ForegroundColorIndex,
        HighlightColorIndex = AutomaticColorIndex,
        Size = 16,
        LineHeight = 1.5
    };

    public static readonly RtfFormat Bold = new() { Bold = true };
    public static readonly RtfFormat Regular = new() { Bold = false };

    public static readonly RtfFormat Highlight = new()
    {
        Bold = false,
        Italic = false,
        ForeColorIndex = 3,
        HighlightColorIndex = 7,
        Size = 12,
        FontIndex = 0,
        LineHeight = 1
    };
}
