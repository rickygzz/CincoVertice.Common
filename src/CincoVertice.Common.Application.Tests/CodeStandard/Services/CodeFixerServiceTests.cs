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

    [Fact]
    public void FixFolder_WithSomeFixers_RunsOnlyThoseInEveryLanguage()
    {
        // Arrange: a comment without its space, and spaces at the end of a line, in C# and JavaScript
        string cs = CreateFile("a.cs", "x(); //a   \n");
        string js = CreateFile("b.js", "x(); //b   \n");
        var fixer = new CodeFixerService(CodeLanguageModel.All);

        // Act
        fixer.FixFolder(_folder, new FixOptionsModel(["TrailingWhitespace"]));

        // Assert: the spaces are gone, the comments as they were
        Assert.Equal("x(); //a\n", File.ReadAllText(cs));
        Assert.Equal("x(); //b\n", File.ReadAllText(js));
    }

    [Fact]
    public void FixFolder_WithNoFixers_ChangesNothing()
    {
        // Arrange
        string path = CreateFile("a.cs", "x(); //a   \n");

        // Act
        List<FileFixResultModel> results =
            new CodeFixerService(CodeLanguageModel.All).FixFolder(_folder, new FixOptionsModel([]));

        // Assert
        Assert.False(Assert.Single(results).IsChanged);
        Assert.Equal("x(); //a   \n", File.ReadAllText(path));
    }

    [Theory]
    [InlineData(LineEndingEnum.CRLF, "// a\r\n// b\r\n// c\r\n")]
    [InlineData(LineEndingEnum.LF, "// a\n// b\n// c\n")]
    [InlineData(LineEndingEnum.CR, "// a\r// b\r// c\r")]
    public void FixFile_WithALineEnding_GivesItToEveryLine_AfterTheFixers(LineEndingEnum lineEnding, string expected)
    {
        // Arrange: a mix of the three
        string path = CreateFile("a.cs", "//a\r\n//b\n//c\r");

        // Act
        FileFixResultModel result = _fixer.FixFile(path, new FixOptionsModel(LineEnding: lineEnding));

        // Assert
        Assert.True(result.IsChanged);
        Assert.Equal(expected, File.ReadAllText(path));
    }

    [Theory]
    [InlineData(false, FileEncodingEnum.Utf8Bom, true)]
    [InlineData(true, FileEncodingEnum.Utf8, false)]
    [InlineData(true, FileEncodingEnum.Keep, true)]
    [InlineData(false, FileEncodingEnum.Keep, false)]
    public void FixFile_WithAnEncoding_WritesTheFileSo_EvenWithNothingElseToFix(
        bool hasBom,
        FileEncodingEnum encoding,
        bool expectBom)
    {
        // Arrange: nothing for the fixer to change, and a character UTF-8 writes in two bytes
        string path = Path.Combine(_folder, "a.cs");
        byte[] text = Encoding.UTF8.GetBytes("// é\n");
        File.WriteAllBytes(path, hasBom ? [.. _utf8Bom, .. text] : text);

        // Act
        FileFixResultModel result = _fixer.FixFile(path, new FixOptionsModel(Encoding: encoding));

        // Assert: written only when the BOM changes
        Assert.Equal(hasBom != expectBom, result.IsChanged);
        Assert.Equal(expectBom ? [.. _utf8Bom, .. text] : text, File.ReadAllBytes(path));
    }

    [Fact]
    public void FixFile_AUtf16File_BecomesUtf8_WhenAsked()
    {
        // Arrange
        string path = Path.Combine(_folder, "a.cs");
        File.WriteAllText(path, "// é\n", Encoding.Unicode);

        // Act
        FileFixResultModel result = _fixer.FixFile(path, new FixOptionsModel(Encoding: FileEncodingEnum.Utf8Bom));

        // Assert
        Assert.True(result.IsChanged);
        Assert.Equal([.. _utf8Bom, .. Encoding.UTF8.GetBytes("// é\n")], File.ReadAllBytes(path));
    }

    [Fact]
    public void FixFolder_WithNoFixers_ButALineEnding_OnlyConverts()
    {
        // Arrange
        string path = CreateFile("a.cs", "//a   \n");

        // Act
        new CodeFixerService(CodeLanguageModel.All).FixFolder(
            _folder,
            new FixOptionsModel([], LineEndingEnum.CRLF));

        // Assert: the comment and the spaces as they were
        Assert.Equal("//a   \r\n", File.ReadAllText(path));
    }

    [Theory]
    [InlineData(null, LineEndingEnum.None, FileEncodingEnum.Keep, true)]
    [InlineData(new string[0], LineEndingEnum.None, FileEncodingEnum.Keep, false)]
    [InlineData(new string[0], LineEndingEnum.LF, FileEncodingEnum.Keep, true)]
    [InlineData(new string[0], LineEndingEnum.None, FileEncodingEnum.Utf8Bom, true)]
    public void Options_DoNothing_OnlyWithNoFixersAndKeepingTheRest(
        string[]? fixers,
        LineEndingEnum lineEnding,
        FileEncodingEnum encoding,
        bool doesAnything)
    {
        Assert.Equal(doesAnything, new FixOptionsModel(fixers, lineEnding, encoding).DoesAnything);
    }

    private string CreateFile(string relativePath, string content)
    {
        string path = Path.Combine(_folder, relativePath);
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, content);

        return path;
    }
}
