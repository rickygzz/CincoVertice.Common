namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     Ends a script early without an error: thrown by stop(), and by ask() when the user cancels.
/// </summary>
public class ScriptStopException : Exception
{
}
