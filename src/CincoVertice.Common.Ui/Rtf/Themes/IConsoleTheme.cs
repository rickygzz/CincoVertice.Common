using CincoVertice.Common.Ui.Rtf.Models;

namespace CincoVertice.Common.Ui.Rtf.Themes;

public interface IConsoleTheme
{
    RtfColor Background { get; set; }

    Dictionary<string, string> Colors { get; set; }
}
