namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Finds the files that the code standard applies to, so the checker and the fixer cover the same files.
///     Build output (bin, obj), hidden tool folders (.git, .vs), installed packages (node_modules) and generated
///     files (*.Designer.cs, *.g.cs, *.min.js) are skipped.
/// </summary>
public static class SourceFileEnumerator
{
    private static readonly string[] _skippedFolders = ["bin", "obj", ".git", ".vs", "node_modules"];

    private static readonly string[] _generatedFileSuffixes = [".Designer.cs", ".g.cs", ".g.i.cs", ".min.js"];

    /// <summary>
    ///     C# source files in the folder and its subfolders, sorted by path.
    /// </summary>
    /// <exception cref="DirectoryNotFoundException">The folder does not exist.</exception>
    public static List<string> GetSourceFiles(string folderPath) => GetSourceFiles(folderPath, [".cs"]);

    /// <summary>
    ///     Source files with one of the extensions in the folder and its subfolders, sorted by path.
    /// </summary>
    /// <param name="folderPath">The folder.</param>
    /// <param name="extensions">Extensions with the dot, such as ".cs", in any letter case.</param>
    /// <exception cref="DirectoryNotFoundException">The folder does not exist.</exception>
    public static List<string> GetSourceFiles(string folderPath, IEnumerable<string> extensions)
    {
        if (!Directory.Exists(folderPath))
        {
            throw new DirectoryNotFoundException($"Folder not found: {folderPath}");
        }

        var wanted = new HashSet<string>(extensions, StringComparer.OrdinalIgnoreCase);

        return [.. EnumerateSourceFiles(folderPath, wanted).Order(StringComparer.OrdinalIgnoreCase)];
    }

    /// <summary>
    ///     Walks the folder without walking into skipped folders.
    /// </summary>
    private static IEnumerable<string> EnumerateSourceFiles(string folderPath, HashSet<string> extensions)
    {
        EnumerationOptions options = new() { IgnoreInaccessible = true };
        Stack<string> folders = new([folderPath]);

        while (folders.Count > 0)
        {
            string folder = folders.Pop();

            // The extension is compared whole: a *.cs pattern would also match *.csx
            foreach (string filePath in Directory.EnumerateFiles(folder, "*", options))
            {
                if (extensions.Contains(Path.GetExtension(filePath)) && !IsGeneratedFile(filePath))
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
