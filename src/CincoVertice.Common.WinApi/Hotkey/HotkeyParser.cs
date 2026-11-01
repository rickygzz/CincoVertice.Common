using CincoVertice.Common.WinApi.Libs.Enums;

namespace CincoVertice.Common.WinApi.Hotkey;

/// <summary>
///     Reads and writes shortcuts as text, e.g. "Ctrl+Alt+M" or "Shift+F5".
///     <para>Modifiers: Ctrl (or Control), Alt, Shift, Win. Keys: A-Z, 0-9, F1-F24, NumPad0-NumPad9, names such as
///     Space, Enter, Esc, Tab, Delete, Home, PageUp, Up, PrintScreen, or any <see cref="KeyCode"/> name with or
///     without "VK_". Case is ignored. A shortcut needs a modifier, except for the function keys.</para>
/// </summary>
public static class HotkeyParser
{
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
    public static bool TryParse(string? text, out FSModifiers modifiers, out KeyCode key, out string error)
    {
        modifiers = 0;
        key = 0;
        error = string.Empty;

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

            if (!TryParseKey(part, out key))
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

        if (modifiers == 0 && key is < KeyCode.VK_F1 or > KeyCode.VK_F24)
        {
            error = $"'{text}' needs a modifier (Ctrl, Alt, Shift or Win).";

            return false;
        }

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
