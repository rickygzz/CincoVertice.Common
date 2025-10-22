using CincoVertice.Common.Ui.Rtf.Models;

namespace CincoVertice.Common.Ui.Rtf.Helpers;

public static class RtfColorHelper
{
    public static (RtfColor Darker, RtfColor Dark, RtfColor Base, RtfColor Light, RtfColor Lighter) GenerateShades(
        this RtfColor baseColor,
        double darkT = 0.25,
        double darkerT = 0.5,
        double lightT = 0.25,
        double lighterT = 0.5)
    {
        return (
            baseColor.Darken(darkerT),
            baseColor.Darken(darkT),
            baseColor,
            baseColor.Lighten(lightT),
            baseColor.Lighten(lighterT));
    }

    /// <summary>
    ///     Makes the color lighter by mixing it with white in linear-light space.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="t">The mix intensity. 0 = no change, 1 = full white.</param>
    /// <returns>A lighter RtfColor.</returns>
    public static RtfColor MixWithWhiteLinear(RtfColor color, double t)
    {
        t = ClampMin0Max1(t);

        // Mix in linear-light space toward white (1,1,1)
        (double r, double g, double b) = ToLinear01(color);

        r = LinearInterpolation(r, 1.0, t);
        g = LinearInterpolation(g, 1.0, t);
        b = LinearInterpolation(b, 1.0, t);

        return FromLinear01(r, g, b);
    }

    /// <summary>
    ///     Makes the color lighter by mixing it with white in linear-light space.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <param name="t">The mix intensity. 0 = no change, 1 = full black.</param>
    /// <returns>A darker RtfColor.</returns>
    public static RtfColor MixWithBlackLinear(RtfColor color, double t)
    {
        t = ClampMin0Max1(t);

        // Mix in linear-light space toward black (0,0,0)
        (double r, double g, double b) = ToLinear01(color);

        r = LinearInterpolation(r, 0.0, t);
        g = LinearInterpolation(g, 0.0, t);
        b = LinearInterpolation(b, 0.0, t);

        return FromLinear01(r, g, b);
    }

    private static (double r, double g, double b) ToLinear01(RtfColor c)
    {
        return (
            SRgbToLinear01(c.Red / 255.0),
            SRgbToLinear01(c.Green / 255.0),
            SRgbToLinear01(c.Blue / 255.0));
    }

    private static RtfColor FromLinear01(double r, double g, double b)
    {
        return new(
            (int)Math.Round(LinearToSRgb01(ClampMin0Max1(r)) * 255.0),
            (int)Math.Round(LinearToSRgb01(ClampMin0Max1(g)) * 255.0),
            (int)Math.Round(LinearToSRgb01(ClampMin0Max1(b)) * 255.0));
    }

    // sRGB <-> Linear conversions (IEC 61966-2-1)
    private static double SRgbToLinear01(double c)
    {
        return c <= 0.04045 ? c / 12.92 : Math.Pow((c + 0.055) / 1.055, 2.4);
    }

    private static double LinearToSRgb01(double c)
    {
        return c <= 0.0031308 ? 12.92 * c : 1.055 * Math.Pow(c, 1.0 / 2.4) - 0.055;
    }

    private static double LinearInterpolation(double start, double end, double faction)
    {
        return start + (end - start) * faction;
    }

    private static double ClampMin0Max1(double value)
    {
        if (value < 0)
        {
            return 0;
        }
        
        if (value > 1)
        {
            return 1;
        }
        
        return value;
    }
}
