namespace CincoVertice.Common.Ui.Rtf.Models;

/// <summary>
///     Formatting applied to a piece of RTF text. A null property leaves that formatting unchanged
///     (keeps whatever the previous text used).
/// </summary>
public class RtfFormat
{
    public bool? Bold { get; set; }

    public bool? Italic { get; set; }

    public int? ForeColorIndex { get; set; }

    public int? HighlightColorIndex { get; set; }

    public float? Size { get; set; }

    public int? FontIndex { get; set; }

    public double? LineHeight { get; set; }
}
