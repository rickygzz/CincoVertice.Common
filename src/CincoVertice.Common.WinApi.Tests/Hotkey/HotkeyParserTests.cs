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
