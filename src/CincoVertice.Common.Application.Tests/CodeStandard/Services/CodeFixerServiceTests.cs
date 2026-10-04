using System.Text;
using CincoVertice.Common.Application.CodeStandard.Fixers;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeStandard.Services;

public class CodeFixerServiceTests : IDisposable
{
    private static readonly byte[] _utf8Bom = [0xEF, 0xBB, 0xBF];

    // One fixer, so each test knows exactly what changes
    private readonly CodeFixerService _fixer = new(new FixerPipeline([new CommentSpaceFixer()]));
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "CodeFixerServiceTests_" + Guid.NewGuid());

    public CodeFixerServiceTests()
    {
        Directory.CreateDirectory(_folder);
    }

    public void Dispose()
    {
        foreach (string file in Directory.EnumerateFiles(_folder, "*", SearchOption.AllDirectories))
        {
            File.SetAttributes(file, FileAttributes.Normal);
        }

        Directory.Delete(_folder, recursive: true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void FixFile_WhenFileDoesNotExist_Throws()
    {
        Assert.Throws<FileNotFoundException>(() => _fixer.FixFile(Path.Combine(_folder, "missing.cs")));
    }

    [Fact]
    public void FixFile_WhenFixersChangeContent_WritesFileAndReportsChanged()
    {
        // Arrange
        string path = CreateFile("a.cs", "//a\nx();\n");

        // Act
        FileFixResultModel result = _fixer.FixFile(path);

        // Assert
        Assert.True(result.IsChanged);
        Assert.Null(result.SkippedReason);
        Assert.Equal(path, result.FilePath);
        Assert.Equal("// a\nx();\n", File.ReadAllText(path));
    }

    [Fact]
    public void FixFile_WhenNothingToFix_DoesNotWriteFile()
    {
        // Arrange
        string path = CreateFile("a.cs", "// a\n");
        var past = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        File.SetLastWriteTimeUtc(path, past);

        // Act
        FileFixResultModel result = _fixer.FixFile(path);

        // Assert
        Assert.False(result.IsChanged);
        Assert.Equal(past, File.GetLastWriteTimeUtc(path));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FixFile_KeepsWhetherFileHasUtf8Bom(bool hasBom)
    {
        // Arrange
        byte[] text = Encoding.UTF8.GetBytes("//café\n");
        string path = Path.Combine(_folder, "a.cs");
        File.WriteAllBytes(path, hasBom ? [.. _utf8Bom, .. text] : text);

        // Act
        _fixer.FixFile(path);

        // Assert
        byte[] expectedText = Encoding.UTF8.GetBytes("// café\n");
        Assert.Equal(hasBom ? [.. _utf8Bom, .. expectedText] : expectedText, File.ReadAllBytes(path));
    }

    [Fact]
    public void FixFile_KeepsCrlfLineEndings()
    {
        // Arrange
        string path = CreateFile("a.cs", "//a\r\n//b\r\n");

        // Act
        _fixer.FixFile(path);

        // Assert
        Assert.Equal("// a\r\n// b\r\n", File.ReadAllText(path));
    }

    [Fact]
    public void FixFile_WhenFileIsNotValidUtf8_SkipsItAndLeavesItUntouched()
    {
        // Arrange: é as a single Latin-1 byte (0xE9), which is not valid UTF-8
        byte[] bytes = Encoding.Latin1.GetBytes("//café\n");
        string path = Path.Combine(_folder, "a.cs");
        File.WriteAllBytes(path, bytes);

        // Act
        FileFixResultModel result = _fixer.FixFile(path);

        // Assert
        Assert.False(result.IsChanged);
        Assert.NotNull(result.SkippedReason);
        Assert.Equal(bytes, File.ReadAllBytes(path));
    }

    [Fact]
    public void FixFile_WhenFileIsReadOnly_SkipsIt()
    {
        // Arrange
        string path = CreateFile("a.cs", "//a\n");
        File.SetAttributes(path, FileAttributes.ReadOnly);

        // Act
        FileFixResultModel result = _fixer.FixFile(path);

        // Assert
        Assert.False(result.IsChanged);
        Assert.NotNull(result.SkippedReason);
        Assert.Equal("//a\n", File.ReadAllText(path));
    }

    [Fact]
    public void FixFolder_WhenFolderDoesNotExist_Throws()
    {
        Assert.Throws<DirectoryNotFoundException>(() => _fixer.FixFolder(Path.Combine(_folder, "missing")));
    }

    [Fact]
    public void FixFolder_FixesSourceFilesAndSkipsBuildOutputAndGeneratedFiles()
    {
        // Arrange
        string a = CreateFile("a.cs", "//a\n");
        string b = CreateFile(Path.Combine("Sub", "b.cs"), "// b\n");
        string binFile = CreateFile(Path.Combine("bin", "c.cs"), "//c\n");
        string designer = CreateFile("Form.Designer.cs", "//d\n");

        // Act
        List<FileFixResultModel> results = _fixer.FixFolder(_folder);

        // Assert
        Assert.Equal([(a, true), (b, false)], results.Select(r => (r.FilePath, r.IsChanged)));
        Assert.Equal("//c\n", File.ReadAllText(binFile));
        Assert.Equal("//d\n", File.ReadAllText(designer));
    }

    [Fact]
    public void FixFolder_WithDefaultPipeline_LeavesNoFixableIssues()
    {
        // Arrange
        CreateFile(
            "a.cs",
            "public class A\n{\n    //Comment   \n\n\n    public int X(int a)\n    {\n        if(a > 0) return 1;\n"
            + "\n        return 0;\n    }\n\n}\n");
        var checker = new CodeCheckerService(new LineCheckerService());
        int issuesBefore = checker.CheckFolder(_folder).Sum(r => r.Errors.Count);

        // Act
        new CodeFixerService(FixerPipeline.CreateFormatting()).FixFolder(_folder);

        // Assert
        Assert.True(issuesBefore > 0);
        Assert.Empty(checker.CheckFolder(_folder).SelectMany(r => r.Errors));
    }

    private string CreateFile(string relativePath, string content)
    {
        string path = Path.Combine(_folder, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);

        return path;
    }
}
