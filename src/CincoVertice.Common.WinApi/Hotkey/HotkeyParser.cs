using CincoVertice.Common.WinApi.Libs.Enums;

namespace CincoVertice.Common.WinApi.Hotkey;

/// <summary>
///     Reads and writes shortcuts as text, e.g. "Ctrl+Alt+M" or "Shift+F5".
///     <para>Modifiers: Ctrl (or Control), Alt, Shift, Win. Keys: A-Z, 0-9, F1-F24, NumPad0-NumPad9, names such as
///     Space, Enter, Esc, Tab, Delete, Home, PageUp, Up, PrintScreen, or any <see cref="KeyCode"/> name with or
///     without "VK_". A key can also be the character it types on the current keyboard layout, such as "Ctrl+Alt+&lt;";
///     Shift is added when the layout needs it. Case is ignored. A shortcut needs a modifier, except for the function
///     keys.</para>
/// </summary>
public static class HotkeyParser
{
    // Shift state bits in the high byte of VkKeyScan's result
    private const int ShiftStateShift = 1;
    private const int ShiftStateCtrl = 2;
    private const int ShiftStateAlt = 4;

    private static readonly Dictionary<string, FSModifiers> _modifiers = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Ctrl"] = FSModifiers.MOD_CONTROL,
        ["Control"] = FSModifiers.MOD_CONTROL,
        ["Alt"] = FSModifiers.MOD_ALT,
        ["Shift"] = FSModifiers.MOD_SHIFT,
        ["Win"] = FSModifiers.MOD_WIN,
        ["Windows"] = FSModifiers.MOD_WIN,
    };

    private static readonly Dictionary<string, KeyCode> _keyNames = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Space"] = KeyCode.VK_SPACE,
        ["Enter"] = KeyCode.VK_RETURN,
        ["Return"] = KeyCode.VK_RETURN,
        ["Esc"] = KeyCode.VK_ESCAPE,
        ["Escape"] = KeyCode.VK_ESCAPE,
        ["Tab"] = KeyCode.VK_TAB,
        ["Backspace"] = KeyCode.VK_BACK,
        ["Del"] = KeyCode.VK_DELETE,
        ["Delete"] = KeyCode.VK_DELETE,
        ["Ins"] = KeyCode.VK_INSERT,
        ["Insert"] = KeyCode.VK_INSERT,
        ["Home"] = KeyCode.VK_HOME,
        ["End"] = KeyCode.VK_END,
        ["PageUp"] = KeyCode.VK_PRIOR,
        ["PgUp"] = KeyCode.VK_PRIOR,
        ["PageDown"] = KeyCode.VK_NEXT,
        ["PgDn"] = KeyCode.VK_NEXT,
        ["Up"] = KeyCode.VK_UP,
        ["Down"] = KeyCode.VK_DOWN,
        ["Left"] = KeyCode.VK_LEFT,
        ["Right"] = KeyCode.VK_RIGHT,
        ["PrintScreen"] = KeyCode.VK_SNAPSHOT,
        ["PrtSc"] = KeyCode.VK_SNAPSHOT,
        ["Pause"] = KeyCode.VK_PAUSE,
        ["Plus"] = KeyCode.VK_OEM_PLUS,
        ["Minus"] = KeyCode.VK_OEM_MINUS,
    };

    // A modifier key can't be the key of a shortcut
    private static readonly KeyCode[] _modifierKeys =
    [
        KeyCode.VK_SHIFT, KeyCode.VK_CONTROL, KeyCode.VK_MENU, KeyCode.VK_LWIN, KeyCode.VK_RWIN,
        KeyCode.VK_LSHIFT, KeyCode.VK_RSHIFT, KeyCode.VK_LCONTROL, KeyCode.VK_RCONTROL, KeyCode.VK_LMENU,
        KeyCode.VK_RMENU,
    ];

    /// <summary>
    ///     Parses a shortcut such as "Ctrl+Alt+M".
    /// </summary>
    /// <param name="text">The shortcut; spaces around the parts are ignored.</param>
    /// <param name="modifiers">The modifier keys.</param>
    /// <param name="key">The key.</param>
    /// <param name="error">Why the text is not a shortcut; empty when it is.</param>
    /// <returns>True when the text is a valid shortcut.</returns>
    public static bool TryParse(string? text, out FSModifiers modifiers, out KeyCode key, out string error) =>
        TryParse(text, c => Libs.User32.VkKeyScan(c), out modifiers, out key, out error);

    /// <summary>
    ///     Parses a shortcut such as "Ctrl+Alt+M", finding the key of a character such as "&lt;" with
    ///     <paramref name="keyScan"/> instead of the current keyboard layout, e.g. in tests.
    /// </summary>
    /// <param name="text">The shortcut; spaces around the parts are ignored.</param>
    /// <param name="keyScan">
    ///     Like VkKeyScan: the key of a character in the low byte and its shift state in the high byte; -1 when no
    ///     key types it.
    /// </param>
    /// <param name="modifiers">The modifier keys.</param>
    /// <param name="key">The key.</param>
    /// <param name="error">Why the text is not a shortcut; empty when it is.</param>
    /// <returns>True when the text is a valid shortcut.</returns>
    public static bool TryParse(
        string? text,
        Func<char, short> keyScan,
        out FSModifiers modifiers,
        out KeyCode key,
        out string error) =>
        Parse(text, keyScan, requireModifier: true, out modifiers, out key, out error);

    /// <summary>
    ///     Parses keys to press, such as "Ctrl+V" or "Enter": like a shortcut, but a key alone is allowed.
    /// </summary>
    /// <param name="text">The keys; spaces around the parts are ignored.</param>
    /// <param name="modifiers">The modifier keys.</param>
    /// <param name="key">The key.</param>
    /// <param name="error">Why the text is not a key; empty when it is.</param>
    /// <returns>True when the text names a key.</returns>
    public static bool TryParseKeys(string? text, out FSModifiers modifiers, out KeyCode key, out string error) =>
        Parse(text, c => Libs.User32.VkKeyScan(c), requireModifier: false, out modifiers, out key, out error);

    private static bool Parse(
        string? text,
        Func<char, short> keyScan,
        bool requireModifier,
        out FSModifiers modifiers,
        out KeyCode key,
        out string error)
    {
        modifiers = 0;
        key = 0;
        error = string.Empty;

        // Shift that a character needs on the layout, e.g. "<" on a US keyboard; not a modifier the user typed
        FSModifiers characterShift = 0;

        if (string.IsNullOrWhiteSpace(text))
        {
            error = "No shortcut.";

            return false;
        }

        string[] parts = text.Split('+', StringSplitOptions.TrimEntries);
        bool hasKey = false;

        foreach (string part in parts)
        {
            if (part.Length == 0)
            {
                error = $"'{text}' has an empty part; use Plus for the + key.";

                return false;
            }

            if (_modifiers.TryGetValue(part, out FSModifiers modifier))
            {
                modifiers |= modifier;

                continue;
            }

            if (hasKey)
            {
                error = $"'{text}' has more than one key.";

                return false;
            }

            if (part.Length == 1 && !char.IsAsciiLetterOrDigit(part[0]))
            {
                if (!TryParseCharacter(part[0], keyScan, out key, out characterShift, out error))
                {
                    return false;
                }
            }
            else if (!TryParseKey(part, out key))
            {
                error = $"'{part}' is not a key.";

                return false;
            }

            hasKey = true;
        }

        if (!hasKey)
        {
            error = $"'{text}' has no key, only modifiers.";

            return false;
        }

        if (requireModifier && modifiers == 0 && key is < KeyCode.VK_F1 or > KeyCode.VK_F24)
        {
            error = $"'{text}' needs a modifier (Ctrl, Alt, Shift or Win).";

            return false;
        }

        modifiers |= characterShift;

        return true;
    }

    /// <summary>
    ///     Writes a shortcut the way <see cref="TryParse"/> reads it, e.g. "Ctrl+Alt+M".
    /// </summary>
    public static string Format(FSModifiers modifiers, KeyCode key)
    {
        List<string> parts = [];

        if (modifiers.HasFlag(FSModifiers.MOD_CONTROL))
        {
            parts.Add("Ctrl");
        }

        if (modifiers.HasFlag(FSModifiers.MOD_ALT))
        {
            parts.Add("Alt");
        }

        if (modifiers.HasFlag(FSModifiers.MOD_SHIFT))
        {
            parts.Add("Shift");
        }

        if (modifiers.HasFlag(FSModifiers.MOD_WIN))
        {
            parts.Add("Win");
        }

        parts.Add(FormatKey(key));

        return string.Join('+', parts);
    }

    /// <summary>
    ///     The key that types <paramref name="c"/> on the layout, and Shift when the character needs it: "&lt;" is
    ///     Shift+OEM_COMMA on a US keyboard and OEM_102 on a Spanish one.
    /// </summary>
    private static bool TryParseCharacter(
        char c,
        Func<char, short> keyScan,
        out KeyCode key,
        out FSModifiers shift,
        out string error)
    {
        key = 0;
        shift = 0;
        error = string.Empty;

        short scan = keyScan(c);

        if (scan == -1)
        {
            error = $"No key types '{c}' on the current keyboard layout.";

            return false;
        }

        int shiftState = (scan >> 8) & 0xFF;

        // AltGr is Ctrl+Alt to Windows: a hotkey on that key would stop the character from being typed anywhere
        if ((shiftState & (ShiftStateCtrl | ShiftStateAlt)) != 0)
        {
            error = $"'{c}' is typed with AltGr, and a hotkey on its key would stop you from typing it.";

            return false;
        }

        key = (KeyCode)(scan & 0xFF);

        if ((shiftState & ShiftStateShift) != 0)
        {
            shift = FSModifiers.MOD_SHIFT;
        }

        return true;
    }

    private static bool TryParseKey(string name, out KeyCode key)
    {
        key = 0;

        // Only names: Enum.TryParse would also accept numbers, such as "65" for A
        if (!name.All(c => char.IsAsciiLetterOrDigit(c) || c == '_'))
        {
            return false;
        }

        if (name.Length == 1 && char.IsAsciiDigit(name[0]))
        {
            key = (KeyCode)((int)KeyCode.D0 + (name[0] - '0'));

            return true;
        }

        if (_keyNames.TryGetValue(name, out key))
        {
            return true;
        }

        bool found = (!name.All(char.IsAsciiDigit)
            && Enum.TryParse(name, ignoreCase: true, out key))
            || Enum.TryParse("VK_" + name, ignoreCase: true, out key);

        return found && Enum.IsDefined(key) && !_modifierKeys.Contains(key);
    }

    private static string FormatKey(KeyCode key)
    {
        if (key is >= KeyCode.D0 and <= KeyCode.D9)
        {
            return ((char)('0' + (key - KeyCode.D0))).ToString();
        }

        string name = key.ToString();

        return name.StartsWith("VK_", StringComparison.Ordinal) ? name[3..] : name;
    }
}
