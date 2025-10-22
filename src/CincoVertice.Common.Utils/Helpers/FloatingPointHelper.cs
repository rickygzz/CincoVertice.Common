namespace CincoVertice.Common.Utils.Helpers;

public static class FloatingPointHelper
{
    /// <summary>
    ///     Determines if 2 floating point values are equal considering a tolerance
    /// </summary>
    /// <param name="value1">The first double value.</param>
    /// <param name="value2">The second double value.</param>
    /// <param name="tolerance">
    ///     Optional. The tolerance within which the values are considered the same. Default is 0.0001.
    /// </param>
    /// <returns>True if the values are equal, otherwise false.</returns>
    public static bool AreEqual(double? value1, double? value2, double tolerance = 0.0001)
    {
        if (value1.HasValue && value2.HasValue)
        {
            return Math.Abs(value1!.Value - value2!.Value) < tolerance;
        }

        if (!value1.HasValue && !value2.HasValue)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Determines if 2 floating point values are equal considering a tolerance
    /// </summary>
    /// <param name="value1">The first double value.</param>
    /// <param name="value2">The second double value.</param>
    /// <param name="tolerance">
    ///     Optional. The tolerance within which the values are considered the same. Default is 0.0001.
    /// </param>
    /// <returns>True if the values are equal, otherwise false.</returns>
    public static bool AreEqual(float? value1, float? value2, float tolerance = 0.0001f)
    {
        if (value1.HasValue && value2.HasValue)
        {
            return Math.Abs(value1!.Value - value2!.Value) < tolerance;
        }

        if (!value1.HasValue && !value2.HasValue)
        {
            return true;
        }

        return false;
    }
}
