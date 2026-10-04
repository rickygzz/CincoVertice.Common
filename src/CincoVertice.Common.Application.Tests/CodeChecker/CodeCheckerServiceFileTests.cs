using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Models;
using CincoVerticeCommon.Application.CodeChecker;
using Xunit;

namespace CincoVertice.Common.Application.Tests.CodeChecker;

public class CodeCheckerServiceFileTests : IDisposable
{
    private readonly CodeCheckerService _checker = new();
    private readonly List<string> _tempFiles = [];

    [Fact]
    public void CheckFile_WhenFileIsEmpty_ReturnsNoErrors()
    {
        // Arrange
        string filePath = CreateTempFile(string.Empty);

        // Act
        _checker.CheckFile(filePath, out List<ErrorModel> errors);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void CheckFile_WhenFileIsClean_ReturnsNoErrors()
    {
        // Arrange
        string[] lines =
        [
            "public class Test",
            "{",
            "    public int Value { get; set; }",
            "}"
        ];
        string filePath = CreateTempFile(string.Join('\n', lines));

        // Act
        _checker.CheckFile(filePath, out List<ErrorModel> errors);

        // Assert
        Assert.Empty(errors);
    }

    [Fact]
    public void CheckFile_WhenLineHasError_ReportsErrorWithCorrectLineNumber()
    {
        // Arrange
        string[] lines =
        [
            "public class Test",       // line 1 - clean
            "    var a = 1;   ",       // line 2 - trailing whitespace (CH0011)
            "}"                        // line 3 - clean
        ];
        string filePath = CreateTempFile(string.Join('\n', lines));

        // Act
        _checker.CheckFile(filePath, out List<ErrorModel> errors);

        // Assert
        var error = Assert.Single(errors);
        Assert.Equal(nameof(Errors.CH0011), error.Code);
        Assert.Equal(2, error.Line);
        Assert.Equal(Errors.CH0011, error.Message);
    }

    [Fact]
    public void CheckFile_WhenMultipleLinesHaveErrors_AggregatesAllErrors()
    {
        // Arrange
        string[] lines =
        [
            "trailing whitespace here ", // line 1 - CH0011
            new string('a', 121)         // line 2 - CH0016 (over 120 chars)
        ];
        string filePath = CreateTempFile(string.Join('\n', lines));

        // Act
        _checker.CheckFile(filePath, out List<ErrorModel> errors);

        // Assert
        Assert.Equal(2, errors.Count);
        Assert.Contains(errors, e => e.Code == nameof(Errors.CH0011) && e.Line == 1);
        Assert.Contains(errors, e => e.Code == nameof(Errors.CH0016) && e.Line == 2);
    }

    [Fact]
    public void CheckFile_NestedIndentationWithBlankLines_DoesNotReportCh0014()
    {
        // Arrange - each level increases by exactly four spaces, with blank lines between blocks.
        string[] lines =
        [
            "public class Test",                 // level 0
            "{",                                 // level 0
            "    public void Configure()",       // level 1
            "    {",                             // level 1
            "",                                  // blank
            "        Builder.Entity<Foo>()",     // level 2
            "            .HasIndex(x)",          // level 3
            "            .IsOnline();",          // level 3
            "",                                  // blank
            "        Builder.Entity<Bar>()",     // level 2
            "            .HasIndex(y);",         // level 3
            "    }",                             // level 1
            "}"                                  // level 0
        ];
        string filePath = CreateTempFile(string.Join("\r\n", lines));

        // Act
        _checker.CheckFile(filePath, out List<ErrorModel> errors);

        // Assert - a one-level increase per line (even across blank lines) is valid.
        Assert.DoesNotContain(errors, e => e.Code == nameof(Errors.CH0014));
    }

    [Fact]
    public void CheckFile_WhenFileCannotBeRead_AddsCh0002Error()
    {
        // Arrange
        string filePath = CreateTempFile("public class Test");

        // Lock the file so the service's StreamReader fails to open it.
        using var _ = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);

        // Act
        _checker.CheckFile(filePath, out List<ErrorModel> errors);

        // Assert
        var error = Assert.Single(errors);
        Assert.Equal(nameof(Errors.CH0002), error.Code);
        Assert.Equal(0, error.Line);
    }

    private string CreateTempFile(string content)
    {
        string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.cs");
        File.WriteAllText(filePath, content);
        _tempFiles.Add(filePath);

        return filePath;
    }

    public void Dispose()
    {
        foreach (var filePath in _tempFiles)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        GC.SuppressFinalize(this);
    }
}
