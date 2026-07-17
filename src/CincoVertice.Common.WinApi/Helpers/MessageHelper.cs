using System.Runtime.Versioning;

namespace CincoVertice.Common.WinApi.Helpers;

[SupportedOSPlatform("windows10.0")]
public class MessageHelper
{
    private static readonly string _errorTitle = "Error";

    private static bool enableMsgBox = true;
    private static System.Text.StringBuilder sb = new(100);
    private static NewMessageCallback? callbackFunction = null;

    public delegate void NewMessageCallback();

    /// <summary>
    /// Activates internal flag.
    /// <para>If value = true it shows MessageBoxes.</para>
    /// <para>If value = false, it creates a buffer to store messages.</para>
    /// <para>Must call Message.Text() to deactivate the flag, and return to normal behavior (show message boxes).</para>
    /// </summary>
    /// <param name="value">If value = true it shows MessageBoxes. Otherwise, it creates a buffer to store messages.</param>
    /// <param name="callback">Callback function, fired every time buffer is updated.</param>
    /// <param name="bufferSize">Optional. Sets the buffer size.</param>
    public static void EnableMsgBox(bool value, NewMessageCallback? callback = null, int bufferSize = 1024 * 300)
    {
        if (enableMsgBox == true && value == false)
        {
            // Value changed, create a buffer.
            sb = new System.Text.StringBuilder(bufferSize);
        }

        callbackFunction = callback;

        enableMsgBox = value;
    }

    /// <summary>
    ///     Get buffered messages.
    /// </summary>
    /// <returns>Buffered messages.</returns>
    public static string BufferToText()
    {
        string messages = sb.ToString();

        sb = new System.Text.StringBuilder(1024 * 300);

        return messages;
    }

    /// <summary>
    /// Displays an Error MessageBox.
    /// </summary>
    /// <returns>The MessageBox.Show() dialog result.</returns>
    /// <param name="errorID">
    ///     A special identifier to identify where the error is coming from (function name, process, etc.).
    ///     May be string.Empty.
    /// </param>
    /// <param name="message">The error message.</param>
    /// <param name="buttons">Buttons.</param>
    public static DialogResult Error(string errorID, string message, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        if (enableMsgBox == false)
        {
            if (string.IsNullOrEmpty(message) == true)
            {
                sb.Append(errorID + System.Environment.NewLine);
            }
            else
            {
                sb.Append(errorID + ": " + message + System.Environment.NewLine);
            }

            callbackFunction?.Invoke();

            return DialogResult.Ignore;
        }
        else
        {
            if (string.IsNullOrEmpty(errorID) == false)
            {
                message = errorID + System.Environment.NewLine + System.Environment.NewLine + message;
            }

            return MessageBox.Show(message, _errorTitle, buttons, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Displays an Warning MessageBox.
    /// </summary>
    /// <param name="warningID">
    ///     A special identifier to identify where the error is coming from (function name, process, etc.). May be string.Empty.
    /// </param>
    /// <param name="message">The error message.</param>
    /// <param name="buttons">buttons.</param>
    /// <returns>The MessageBox.Show() dialog result.</returns>
    public static DialogResult Warning(string warningID, string message, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        if (enableMsgBox == false)
        {
            if (string.IsNullOrEmpty(message) == true)
            {
                sb.Append(warningID + System.Environment.NewLine);
            }
            else
            {
                sb.Append(warningID + ": " + message + System.Environment.NewLine);
            }

            callbackFunction?.Invoke();

            return DialogResult.Ignore;
        }
        else
        {
            if (!string.IsNullOrEmpty(warningID))
            {
                message = warningID + System.Environment.NewLine + System.Environment.NewLine + message;
            }

            return MessageBox.Show(message, _errorTitle, buttons, MessageBoxIcon.Warning);
        }
    }

    /// <summary>
    /// Displays an Information MessageBox.
    /// </summary>
    /// <param name="informationID">
    ///     A special identifier to identify where the error is coming from (function name, process, etc.). May be string.Empty.
    /// </param>
    /// <param name="message">The error message.</param>
    /// <param name="buttons">buttons.</param>
    /// <returns>The MessageBox.Show() dialog result.</returns>
    public static DialogResult Information(string informationID, string message, MessageBoxButtons buttons = MessageBoxButtons.OK)
    {
        if (enableMsgBox == false)
        {
            if (string.IsNullOrEmpty(message) == true)
            {
                sb.Append(informationID + System.Environment.NewLine);
            }
            else
            {
                sb.Append(informationID + ": " + message + System.Environment.NewLine);
            }

            callbackFunction?.Invoke();

            return DialogResult.Ignore;
        }
        else
        {
            if (string.IsNullOrEmpty(informationID) == false)
            {
                message = informationID + System.Environment.NewLine + System.Environment.NewLine + message;
            }

            return MessageBox.Show(message, _errorTitle, buttons, MessageBoxIcon.Information);
        }
    }

    /// <summary>
    /// Show last Win32 error.
    /// </summary>
    /// <param name="informationID">
    ///     A special identifier to identify where the error is coming from (function name, process, etc.). May be string.Empty.
    /// </param>
    /// <returns>The MessageBox.Show() dialog result.</returns>
    public static DialogResult ShowLastWin32Error(string informationID)
    {
        int lastError = System.Runtime.InteropServices.Marshal.GetLastWin32Error();
        System.ComponentModel.Win32Exception win32Exception = new(lastError);

        string message = win32Exception.Message + " (Win32 Error # " + lastError + ")";

        if (!string.IsNullOrEmpty(informationID))
        {
            message = informationID + System.Environment.NewLine + System.Environment.NewLine + message;
        }

        return MessageBox.Show(message, _errorTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
