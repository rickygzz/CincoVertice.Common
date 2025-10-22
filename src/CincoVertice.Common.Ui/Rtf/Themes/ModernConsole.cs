using CincoVertice.Common.Ui.Rtf.Models;

namespace CincoVertice.Common.Ui.Rtf.Themes;

public static class ModernConsole
{
    public static RtfColor Background { get; set; } = new(30, 30, 30);

    public static Dictionary<string, RtfColor> Colors { get; set; } = new()
    {
        { "Background", Background },
        { "Foreground", new RtfColor(255, 255, 255) },
        { "Accent1Base", new RtfColor(78, 103, 200) },
        { "Accent1Lightest", new RtfColor(180, 220, 250).Lighten(0.80) },
        { "Accent1Lighter", new RtfColor(180, 220, 250).Lighten(0.60) },
        { "Accent1Light", new RtfColor(180, 220, 250).Lighten(0.40) },
        { "Accent1Dark", new RtfColor(180, 220, 250).Darken(0.25) }
    };

    public static int ColorIndex(string colorName)
    {
        return Colors.Keys.ToList().IndexOf(colorName);
    }
}
