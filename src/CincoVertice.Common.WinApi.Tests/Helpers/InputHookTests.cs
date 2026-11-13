using System.Windows.Forms;
using CincoVertice.Common.WinApi.Helpers;
using CincoVertice.Common.WinApi.Libs.Enums;
using Xunit;

namespace CincoVertice.Common.WinApi.Tests.Helpers;

/// <summary>With a real hook, and F24, which keyboards don't have and programs don't use.</summary>
public class InputHookTests
{
    [Fact]
    public void AKeyPressed_IsToldOf_UntilDisposed()
    {
        // Arrange
        var keys = new List<Keys>();
        var hook = new InputHook();
        hook.KeyDown += (_, key) => keys.Add(key);

        // Act
        InputSimulator.SendShortcut(0, KeyCode.VK_F24, altGr: false);
        Pump();
        hook.Dispose();
        InputSimulator.SendShortcut(0, KeyCode.VK_F24, altGr: false);
        Pump();

        // Assert
        Assert.Equal([Keys.F24], keys);
    }

    // The hook's calls come through this thread's messages
    private static void Pump()
    {
        for (int i = 0; i < 20; i++)
        {
            Application.DoEvents();
            Thread.Sleep(5);
        }
    }
}
