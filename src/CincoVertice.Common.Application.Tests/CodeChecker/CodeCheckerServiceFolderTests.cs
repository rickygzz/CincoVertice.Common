using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVertice.Common.Application.CodeStandard.Services;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker;

public class CodeCheckerServiceFolderTests : IDisposable
{
    private const string CleanCode = "public class Test\n{\n}";

    private readonly CodeCheckerService _checker = new(new LineCheckerService());
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "CodeCheckerFolderTests_" + Guid.NewGuid());

    public CodeCheckerServiceFolderTests()
    {
        Directory.CreateDirectory(_folder);
    }

    public void Dispose()
    {
        Directory.Delete(_folder, recursive: true);
        GC.SuppressFinalize(this);
    }

    [Fact]
    public void CheckFolder_WhenFolderDoesNotExist_Throws()
    {
        Assert.Throws<DirectoryNotFoundException>(() => _checker.CheckFolder(Path.Combine(_folder, "missing")));
    }

    [Fact]
    public void CheckFolder_WhenFolderIsEmpty_ReturnsNoResults()
    {
        List<FileCheckResultModel> results = _checker.CheckFolder(_folder);

        Assert.Empty(results);
    }

    [Fact]
    public void CheckFolder_ChecksCsFilesInSubfolders_SortedByPath()
    {
        // Arrange
        CreateFile("b.cs", CleanCode);
        CreateFile(Path.Combine("Sub", "a.cs"), CleanCode);
        CreateFile("a.cs", CleanCode);

        // Act
        List<FileCheckResultModel> results = _checker.CheckFolder(_folder);

        // Assert
        Assert.Equal(
            [Path.Combine(_folder, "a.cs"), Path.Combine(_folder, "b.cs"), Path.Combine(_folder, "Sub", "a.cs")],
            results.Select(r => r.FilePath));
        Assert.All(results, r => Assert.Empty(r.Errors));
    }

    [Fact]
    public void CheckFolder_ReportsErrorsPerFile()
    {
        // Arrange
        CreateFile("clean.cs", CleanCode);
        CreateFile("dirty.cs", "public class Test \n{\n}");

        // Act
        List<FileCheckResultModel> results = _checker.CheckFolder(_folder);

        // Assert
        Assert.Empty(results.Single(r => r.FilePath.EndsWith("clean.cs")).Errors);

        ErrorModel error = Assert.Single(results.Single(r => r.FilePath.EndsWith("dirty.cs")).Errors);
        Assert.Equal(nameof(Errors.CH0011), error.Code);
        Assert.Equal(1, error.Line);
    }

    [Theory]
    [InlineData("bin")]
    [InlineData("obj")]
    [InlineData(".git")]
    [InlineData(".vs")]
    [InlineData("OBJ")]
    public void CheckFolder_SkipsBuildAndToolFolders(string skippedFolder)
    {
        CreateFile(Path.Combine(skippedFolder, "skipped.cs"), CleanCode);
        CreateFile(Path.Combine(skippedFolder, "Nested", "skipped.cs"), CleanCode);

        List<FileCheckResultModel> results = _checker.CheckFolder(_folder);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData("Form.Designer.cs")]
    [InlineData("Generated.g.cs")]
    [InlineData("Page.g.i.cs")]
    public void CheckFolder_SkipsGeneratedFiles(string fileName)
    {
        CreateFile(fileName, CleanCode);

        List<FileCheckResultModel> results = _checker.CheckFolder(_folder);

        Assert.Empty(results);
    }

    [Theory]
    [InlineData("script.csx")]
    [InlineData("Project.csproj")]
    [InlineData("readme.txt")]
    public void CheckFolder_SkipsNonCsFiles(string fileName)
    {
        CreateFile(fileName, CleanCode);

        List<FileCheckResultModel> results = _checker.CheckFolder(_folder);

        Assert.Empty(results);
    }

    private void CreateFile(string relativePath, string content)
    {
        string filePath = Path.Combine(_folder, relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllText(filePath, content);
    }
}
