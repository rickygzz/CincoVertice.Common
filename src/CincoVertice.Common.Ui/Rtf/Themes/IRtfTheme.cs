using CincoVertice.Common.Ui.Rtf.Models;

namespace CincoVertice.Common.Ui.Rtf.Themes;

public interface IRtfTheme
{
    string DefaultFont { get; set; }

    float DefaultFontSize { get; set; }

    Dictionary<string, RtfColor> Colors { get; set; }
}
