using CincoVertice.Common.Ui.Rtf.Helpers;

namespace CincoVertice.Common.Ui.Rtf.Models;

public class RtfColor(int red, int green, int blue)
{
    public int Red { get; set; } = red;
    public int Green { get; set; } = green;
    public int Blue { get; set; } = blue;

    /// <summary>
    ///     Lightens the color by mixing it with white by a factor t.
    /// </summary>
    /// <param name="t">Whitening factor from 0 to 1.</param>
    /// <returns>Returns the new color.</returns>
    public RtfColor Lighten(double t)
    {
        return RtfColorHelper.MixWithWhiteLinear(this, t);
    }

    /// <summary>
    ///     Darkens the color by mixing it with black by a factor t.
    /// </summary>
    /// <param name="t">Darkening factor from 0 to 1.</param>
    /// <returns>Returns the new color.</returns>
    public RtfColor Darken(double t)
    {
        return RtfColorHelper.MixWithBlackLinear(this, t);
    }
}
