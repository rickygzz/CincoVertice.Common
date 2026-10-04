namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Finds the C# files that the code standard applies to, so the checker and the fixer cover the same files.
///     Build output (bin, obj), hidden tool folders (.git, .vs) and generated files (*.Designer.cs, *.g.cs) are
///     skipped.
/// </summary>
public static class SourceFileEnumerator
{
    private static readonly string[] _skippedFolders = ["bin", "obj", ".git", ".vs"];

    private static readonly string[] _generatedFileSuffixes = [".Designer.cs", ".g.cs", ".g.i.cs"];

    /// <summary>
    ///     C# source files in the folder and its subfolders, sorted by path.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">The folder does not exist.</exception>
    public static List<string> GetSourceFiles(string folderPath)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
        }

        return [.. EnumerateSourceFiles(folderPath).Order(StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    ///     Walks the folder without walking into skipped folders.
    /// </summary>
    private static IEnumerable<string> EnumerateSourceFiles(string folderPath)
    {
        // The EnumerationOptions overload avoids the legacy pattern match where *.cs also matches *.csx
        EnumerationOptions options = new() { IgnoreInaccessible = true };
        Stack<string> folders = new([folderPath]);

        while (folders.Count > 0)
        {
            string folder = folders.Pop();

            foreach (string filePath in Directory.EnumerateFiles(folder, "*.cs", options))
            {
                if (!IsGeneratedFile(filePath))
                {
                    yield return filePath;
                }
            }

            foreach (string subfolder in Directory.EnumerateDirectories(folder, "*", options))
            {
                if (!_skippedFolders.Contains(Path.GetFileName(subfolder), StringComparer.OrdinalIgnoreCase))
                {
                    folders.Push(subfolder);
                }
            }
        }
    }

    private static bool IsGeneratedFile(string filePath)
    {
        return _generatedFileSuffixes.Any(suffix => filePath.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));
    }
}
