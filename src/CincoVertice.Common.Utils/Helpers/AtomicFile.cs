namespace CincoVertice.Common.Utils.Helpers;

/// <summary>
///     Writes a file through a temporary one next to it, moved into place only once it's complete, so a failure never
///     leaves half a file. Its name is new each time, so it can't be a file that was already there, and it's removed
///     when anything fails.
/// </summary>
public static class AtomicFile
{
    public static void WriteAllText(string path, string text) =>
        Write(path, temporaryPath => File.WriteAllText(temporaryPath, text));

    public static void WriteAllText(string path, string text, System.Text.Encoding encoding) =>
        Write(path, temporaryPath => File.WriteAllText(temporaryPath, text, encoding));

    /// <param name="path">The file to write.</param>
    /// <param name="writeTemporaryFile">Writes the content to the path it's given.</param>
    /// <param name="overwrite">Whether to replace a file already at the path; if not, that fails with IOException.</param>
    public static void Write(string path, Action<string> writeTemporaryFile, bool overwrite = true)
    {
        string temporaryPath = $"{path}.{Guid.NewGuid():N}.tmp";

        try
        {
            writeTemporaryFile(temporaryPath);
            File.Move(temporaryPath, path, overwrite);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }
}
