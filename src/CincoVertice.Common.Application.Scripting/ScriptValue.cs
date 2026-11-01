using System.Globalization;

namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     Script values are decimal (number), string (text), bool and <see cref="IReadOnlyList{T}"/> of values (list),
///     plus <see cref="Nothing"/> for the result of a function that returns no value, such as say().
/// </summary>
public static class ScriptValue
{
    /// <summary>The result of a function that returns no value. It can't be stored or used in an expression.</summary>
    public static readonly object Nothing = new();

    /// <summary>
    ///     The value as text: numbers without trailing zeros and with a . as decimal point, true or false, and lists
    ///     as [a, 1, true].
    /// </summary>
    public static string ToText(object value) => value switch
    {
        string text => text,
        decimal number => number.ToString("0.############################", CultureInfo.InvariantCulture),
        bool flag => flag ? "true" : "false",
        IReadOnlyList<object> list => "[" + string.Join(", ", list.Select(ToText)) + "]",
        _ => string.Empty
    };

    /// <summary>The value's type for error messages: "a number", "a text", "true or false", "a list".</summary>
    public static string Describe(object value) => value switch
    {
        string => "a text",
        decimal => "a number",
        bool => "true or false",
        IReadOnlyList<object> => "a list",
        _ => "no value"
    };

    /// <summary>
    ///     Whether two values are equal: same type and value, and lists item by item. Texts are case-sensitive, and
    ///     a text never equals a number.
    /// </summary>
    public static bool AreEqual(object left, object right)
    {
        if (left is IReadOnlyList<object> leftList && right is IReadOnlyList<object> rightList)
        {
            return leftList.Count == rightList.Count
                && leftList.Zip(rightList).All(pair => AreEqual(pair.First, pair.Second));
        }

        return Equals(left, right);
    }
}
