using CincoVertice.Common.WinApi.Helpers;
using CincoVertice.Common.WinApi.Libs.Enums;
using CincoVertice.Common.WinApi.Libs.Structs;
using Xunit;

namespace CincoVertice.Common.WinApi.Tests.Helpers;

public class InputSimulatorTests
{
    private const KeyboardInputDwFlags Unicode = KeyboardInputDwFlags.KEYEVENTF_UNICODE;
    private const KeyboardInputDwFlags Up = KeyboardInputDwFlags.KEYEVENTF_KEYUP;
    private const KeyboardInputDwFlags Extended = KeyboardInputDwFlags.KEYEVENTF_EXTENDEDKEY;

    [Fact]
    public void UnicodeTextInputs_SendsEachCharacterAsItself()
    {
        // Act: @ is AltGr+2 on a Spanish layout and Shift+2 on an English one, but not here
        INPUT[] inputs = UnicodeText("a@ñ€");

        // Assert: no key, so no layout to read it with
        Assert.Equal(
            [
                (0, 'a', Unicode), (0, 'a', Unicode | Up),
                (0, '@', Unicode), (0, '@', Unicode | Up),
                (0, 'ñ', Unicode), (0, 'ñ', Unicode | Up),
                (0, '€', Unicode), (0, '€', Unicode | Up),
            ],
            Keys(inputs));
        Assert.All(inputs, input => Assert.Equal(InputType.INPUT_KEYBOARD, input.Type));
    }

    [Theory]
    [InlineData("a\nb")]
    [InlineData("a\r\nb")]
    [InlineData("a\rb")]
    public void UnicodeTextInputs_ANewLine_IsOneEnter(string text)
    {
        // Act
        INPUT[] inputs = UnicodeText(text);

        // Assert: a real key, since programs act on Enter rather than on its character
        Assert.Equal(
            [
                (0, 'a', Unicode), (0, 'a', Unicode | Up),
                ((int)KeyCode.VK_RETURN, '\0', 0), ((int)KeyCode.VK_RETURN, '\0', Up),
                (0, 'b', Unicode), (0, 'b', Unicode | Up),
            ],
            Keys(inputs));
    }

    [Fact]
    public void UnicodeTextInputs_ATab_IsTheTabKey()
    {
        // Act
        INPUT[] inputs = UnicodeText("\t");

        // Assert
        Assert.Equal([((int)KeyCode.VK_TAB, '\0', 0), ((int)KeyCode.VK_TAB, '\0', Up)], Keys(inputs));
    }

    [Fact]
    public void UnicodeTextInputs_AnEmoji_GoesAsItsTwoHalves()
    {
        // Arrange: one character, beyond what one UTF-16 unit holds
        const string Smile = "\U0001F600";

        // Act
        INPUT[] inputs = UnicodeText(Smile);

        // Assert
        Assert.Equal(
            [
                (0, Smile[0], Unicode), (0, Smile[0], Unicode | Up),
                (0, Smile[1], Unicode), (0, Smile[1], Unicode | Up),
            ],
            Keys(inputs));
    }

    [Fact]
    public void UnicodeTextInputs_WhatSendKeysGaveAMeaning_IsTypedAsItIs()
    {
        // Act: SendKeys read + as Shift and {ENTER} as Enter
        INPUT[] inputs = UnicodeText("+^%~(){}[]");

        // Assert
        Assert.Equal("+^%~(){}[]", new string([.. inputs.Where((_, i) => i % 2 == 0).Select(Character)]));
    }

    [Fact]
    public void UnicodeTextInputs_NoText_IsNoKeys()
    {
        Assert.Empty(UnicodeText(string.Empty));
    }

    [Fact]
    public void ShortcutInputs_HoldsTheModifiersAroundTheKey()
    {
        // Act
        INPUT[] inputs = InputSimulator.ShortcutInputs(FSModifiers.MOD_CONTROL | FSModifiers.MOD_SHIFT, KeyCode.V);

        // Assert: released in the opposite order
        Assert.Equal(
            [
                ((int)KeyCode.VK_CONTROL, '\0', 0), ((int)KeyCode.VK_SHIFT, '\0', 0),
                ((int)KeyCode.V, '\0', 0), ((int)KeyCode.V, '\0', Up),
                ((int)KeyCode.VK_SHIFT, '\0', Up), ((int)KeyCode.VK_CONTROL, '\0', Up),
            ],
            Keys(inputs));
    }

    [Fact]
    public void ShortcutInputs_WithAltGr_HoldsLeftCtrlAndRightAlt()
    {
        // Act
        INPUT[] inputs = InputSimulator.ShortcutInputs(0, KeyCode.D2, altGr: true);

        // Assert: as a keyboard with AltGr sends it; Right Alt is the extended Alt
        Assert.Equal(
            [
                ((int)KeyCode.VK_LCONTROL, '\0', 0), ((int)KeyCode.VK_RMENU, '\0', Extended),
                ((int)KeyCode.D2, '\0', 0), ((int)KeyCode.D2, '\0', Up),
                ((int)KeyCode.VK_RMENU, '\0', Extended | Up), ((int)KeyCode.VK_LCONTROL, '\0', Up),
            ],
            Keys(inputs));
    }

    [Fact]
    public void ShortcutInputs_WithAltGrAndShift_HoldsAllThree()
    {
        // Act
        INPUT[] inputs = InputSimulator.ShortcutInputs(FSModifiers.MOD_SHIFT, KeyCode.E, altGr: true);

        // Assert
        Assert.Equal(
            [KeyCode.VK_LCONTROL, KeyCode.VK_RMENU, KeyCode.VK_SHIFT],
            inputs.Take(3).Select(input => input.MKHInput.KeyboardInput.WVk));
    }

    [Fact]
    public void ShortcutInputs_SeveralTimes_PressesTheKeyThatOften()
    {
        // Act
        INPUT[] inputs = InputSimulator.ShortcutInputs(0, KeyCode.VK_TAB, times: 3);

        // Assert
        Assert.Equal(6, inputs.Length);
        Assert.All(inputs, input => Assert.Equal(KeyCode.VK_TAB, input.MKHInput.KeyboardInput.WVk));
    }

    private static INPUT[] UnicodeText(string text) => InputSimulator.UnicodeTextInputs(text);

    private static char Character(INPUT input) => (char)input.MKHInput.KeyboardInput.WScan;

    /// <summary>Each key press as its key, the character it carries and its flags.</summary>
    private static IEnumerable<(int Key, char Character, KeyboardInputDwFlags Flags)> Keys(INPUT[] inputs) =>
        inputs.Select(input => (
            (int)input.MKHInput.KeyboardInput.WVk,
            Character(input),
            input.MKHInput.KeyboardInput.DwFlags));
}
