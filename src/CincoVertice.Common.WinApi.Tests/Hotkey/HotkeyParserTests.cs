using CincoVertice.Common.WinApi.Hotkey;
using CincoVertice.Common.WinApi.Libs.Enums;
using Xunit;

namespace CincoVertice.Common.WinApi.Tests.Hotkey;

public class HotkeyParserTests
{
    private const FSModifiers CtrlAlt = FSModifiers.MOD_CONTROL | FSModifiers.MOD_ALT;

    [Theory]
    [InlineData("Ctrl+Alt+M", CtrlAlt, KeyCode.M)]
    [InlineData("ctrl + alt + m", CtrlAlt, KeyCode.M)]
    [InlineData("Control+Alt+A", CtrlAlt, KeyCode.A)]
    [InlineData("Alt+Ctrl+0", CtrlAlt, KeyCode.D0)]
    [InlineData("Shift+F5", FSModifiers.MOD_SHIFT, KeyCode.VK_F5)]
    [InlineData("Win+Space", FSModifiers.MOD_WIN, KeyCode.VK_SPACE)]
    [InlineData("Ctrl+PageUp", FSModifiers.MOD_CONTROL, KeyCode.VK_PRIOR)]
    [InlineData("Ctrl+NumPad7", FSModifiers.MOD_CONTROL, KeyCode.VK_NUMPAD7)]
    [InlineData("Ctrl+VK_OEM_COMMA", FSModifiers.MOD_CONTROL, KeyCode.VK_OEM_COMMA)]
    [InlineData("Ctrl+Plus", FSModifiers.MOD_CONTROL, KeyCode.VK_OEM_PLUS)]
    [InlineData("F12", (FSModifiers)0, KeyCode.VK_F12)]
    public void TryParse_ReadsModifiersAndKey(string text, FSModifiers expectedModifiers, KeyCode expectedKey)
    {
        // Act
        bool ok = HotkeyParser.TryParse(text, out FSModifiers modifiers, out KeyCode key, out string error);

        // Assert
        Assert.True(ok, error);
        Assert.Equal(expectedModifiers, modifiers);
        Assert.Equal(expectedKey, key);
        Assert.Empty(error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Ctrl+Alt")]
    [InlineData("Ctrl+Alt+")]
    [InlineData("Ctrl++")]
    [InlineData("Ctrl+A+B")]
    [InlineData("Ctrl+Foo")]
    [InlineData("Ctrl+65")]
    [InlineData("Ctrl+Shift+VK_SHIFT")]
    [InlineData("Ctrl+F25")]
    [InlineData("M")]
    [InlineData("Space")]
    public void TryParse_WhenNotAShortcut_ReturnsFalseWithError(string? text)
    {
        // Act
        bool ok = HotkeyParser.TryParse(text, out _, out _, out string error);

        // Assert
        Assert.False(ok);
        Assert.NotEmpty(error);
    }

    [Theory]
    [InlineData("Ctrl+Alt+<", "US", CtrlAlt | FSModifiers.MOD_SHIFT, KeyCode.VK_OEM_COMMA)]
    [InlineData("Ctrl+Alt+,", "US", CtrlAlt, KeyCode.VK_OEM_COMMA)]
    [InlineData("Ctrl+Shift+<", "US", FSModifiers.MOD_CONTROL | FSModifiers.MOD_SHIFT, KeyCode.VK_OEM_COMMA)]
    [InlineData("Ctrl+Alt+<", "ES", CtrlAlt, KeyCode.VK_OEM_102)]
    [InlineData("Ctrl+Alt+>", "ES", CtrlAlt | FSModifiers.MOD_SHIFT, KeyCode.VK_OEM_102)]
    [InlineData("Ctrl+ñ", "ES", FSModifiers.MOD_CONTROL, KeyCode.VK_OEM_3)]
    public void TryParse_ReadsCharacterAsTheKeyThatTypesIt(
        string text,
        string layout,
        FSModifiers expectedModifiers,
        KeyCode expectedKey)
    {
        // Act
        bool ok = HotkeyParser.TryParse(text, KeyScan(layout), out var modifiers, out var key, out string error);

        // Assert
        Assert.True(ok, error);
        Assert.Equal((expectedModifiers, expectedKey), (modifiers, key));
    }

    [Theory]
    [InlineData("Ctrl+Alt+@", "ES", "is typed with AltGr")]
    [InlineData("Ctrl+€", "US", "No key types '€'")]
    [InlineData("<", "US", "needs a modifier")]
    [InlineData("Ctrl+<+,", "US", "more than one key")]
    public void TryParse_WhenCharacterCantBeAKey_ReturnsFalseWithError(string text, string layout, string message)
    {
        // Act
        bool ok = HotkeyParser.TryParse(text, KeyScan(layout), out _, out _, out string error);

        // Assert
        Assert.False(ok);
        Assert.Contains(message, error);
    }

    // What VkKeyScan returns for a few characters on a US and a Spanish keyboard: key | shift state << 8
    private static readonly Dictionary<string, Dictionary<char, short>> _layouts = new()
    {
        ["US"] = new() { [','] = 0x00BC, ['<'] = 0x01BC },
        ["ES"] = new() { ['<'] = 0x00E2, ['>'] = 0x01E2, ['ñ'] = 0x00C0, ['@'] = 0x0632 },
    };

    private static Func<char, short> KeyScan(string layout) => c => _layouts[layout].GetValueOrDefault(c, (short)-1);

    [Theory]
    [InlineData("Ctrl+V", FSModifiers.MOD_CONTROL, KeyCode.V)]
    [InlineData("Enter", (FSModifiers)0, KeyCode.VK_RETURN)]
    [InlineData("tab", (FSModifiers)0, KeyCode.VK_TAB)]
    [InlineData("Shift+Down", FSModifiers.MOD_SHIFT, KeyCode.VK_DOWN)]
    public void TryParseKeys_AllowsKeyWithoutModifier(string text, FSModifiers expectedModifiers, KeyCode expectedKey)
    {
        // Act
        bool ok = HotkeyParser.TryParseKeys(text, out var modifiers, out bool altGr, out var key, out string error);

        // Assert
        Assert.True(ok, error);
        Assert.Equal((expectedModifiers, expectedKey), (modifiers, key));
        Assert.False(altGr);
    }

    [Theory]
    [InlineData("")]
    [InlineData("Ctrl")]
    [InlineData("Ctrl+Foo")]
    [InlineData("AltGr")]
    public void TryParseKeys_WhenNotAKey_ReturnsFalseWithError(string text)
    {
        // Act
        bool ok = HotkeyParser.TryParseKeys(text, out _, out _, out _, out string error);

        // Assert
        Assert.False(ok);
        Assert.NotEmpty(error);
    }

    [Theory]
    [InlineData("AltGr+2", (FSModifiers)0, KeyCode.D2)]
    [InlineData("altgr + e", (FSModifiers)0, KeyCode.E)]
    [InlineData("AltGr+Shift+E", FSModifiers.MOD_SHIFT, KeyCode.E)]
    [InlineData("Shift+AltGr+VK_OEM_PLUS", FSModifiers.MOD_SHIFT, KeyCode.VK_OEM_PLUS)]
    public void TryParseKeys_WithAltGr_SaysSo_AndKeepsItOutOfTheModifiers(
        string text,
        FSModifiers expectedModifiers,
        KeyCode expectedKey)
    {
        // Act: the modifiers are RegisterHotKey's, which has no AltGr
        bool ok = HotkeyParser.TryParseKeys(text, out var modifiers, out bool altGr, out var key, out string error);

        // Assert
        Assert.True(ok, error);
        Assert.True(altGr);
        Assert.Equal((expectedModifiers, expectedKey), (modifiers, key));
    }

    [Theory]
    [InlineData("AltGr+Ctrl+2")]
    [InlineData("Alt+AltGr+2")]
    public void TryParseKeys_AltGrWithCtrlOrAlt_IsRefused(string text)
    {
        // Act: AltGr is already Ctrl+Alt, which would be pressed and released in the middle of it
        bool ok = HotkeyParser.TryParseKeys(text, out _, out _, out _, out string error);

        // Assert
        Assert.False(ok);
        Assert.Contains("already Ctrl+Alt", error);
    }

    [Theory]
    [InlineData("Ctrl+AltGr+2")]
    [InlineData("AltGr+F5")]
    public void TryParse_AHotkeyWithAltGr_IsRefused(string text)
    {
        // Act: Windows sees it as Ctrl+Alt, so a hotkey on it would stop the characters it types
        bool ok = HotkeyParser.TryParse(text, out _, out _, out string error);

        // Assert
        Assert.False(ok);
        Assert.Contains("can't be part of a hotkey", error);
    }

    [Fact]
    public void TryParseKeys_ACharacterTypedWithAltGr_SaysHowToWriteIt()
    {
        // Act: pressing its key would type it only on this layout
        bool ok = HotkeyParser.TryParseKeys("@", KeyScan("ES"), out _, out _, out _, out string error);

        // Assert
        Assert.False(ok);
        Assert.Contains("type it as text", error);
    }

    [Fact]
    public void TryParseKeys_ACharacterTypedWithoutAltGr_IsItsKey()
    {
        // Act
        bool ok = HotkeyParser.TryParseKeys(">", KeyScan("ES"), out var modifiers, out bool altGr, out var key, out _);

        // Assert: and the Shift it needs
        Assert.True(ok);
        Assert.Equal((FSModifiers.MOD_SHIFT, false, KeyCode.VK_OEM_102), (modifiers, altGr, key));
    }

    [Theory]
    [InlineData(CtrlAlt, KeyCode.M, "Ctrl+Alt+M")]
    [InlineData(FSModifiers.MOD_SHIFT | FSModifiers.MOD_WIN, KeyCode.VK_F5, "Shift+Win+F5")]
    [InlineData(FSModifiers.MOD_CONTROL, KeyCode.D7, "Ctrl+7")]
    public void Format_WritesShortcut(FSModifiers modifiers, KeyCode key, string expected)
    {
        Assert.Equal(expected, HotkeyParser.Format(modifiers, key));
    }

    [Theory]
    [InlineData("Ctrl+Alt+M")]
    [InlineData("Shift+F5")]
    [InlineData("Ctrl+7")]
    [InlineData("Win+Space")]
    [InlineData("Ctrl+NUMPAD7")]
    public void Format_IsReadBackByTryParse(string text)
    {
        // Arrange
        HotkeyParser.TryParse(text, out FSModifiers modifiers, out KeyCode key, out _);

        // Act
        bool ok = HotkeyParser.TryParse(HotkeyParser.Format(modifiers, key), out var again, out var againKey, out _);

        // Assert
        Assert.True(ok);
        Assert.Equal((modifiers, key), (again, againKey));
    }
}
