using CincoVertice.Common.Utils.Helpers;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Helpers;

public sealed class AtomicFileTest : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "AtomicFileTest_" + Guid.NewGuid());
    private readonly string _path;

    public AtomicFileTest()
    {
        Directory.CreateDirectory(_folder);
        _path = Path.Combine(_folder, "file.txt");
    }

    public void Dispose()
    {
        Directory.Delete(_folder, recursive: true);
    }

    [Fact]
    public void WriteAllText_ReplacesTheFile_LeavingNothingElse()
    {
        // Arrange
        File.WriteAllText(_path, "old");

        // Act
        AtomicFile.WriteAllText(_path, "new");

        // Assert
        Assert.Equal("new", File.ReadAllText(_path));
        Assert.Single(Directory.GetFiles(_folder));
    }

    [Fact]
    public void Write_LeavesAFileNamedLikeATemporaryOne()
    {
        // Arrange
        File.WriteAllText(_path + ".tmp", "mine");

        // Act
        AtomicFile.WriteAllText(_path, "new");

        // Assert
        Assert.Equal("mine", File.ReadAllText(_path + ".tmp"));
    }

    [Fact]
    public void Write_WhenWritingFails_KeepsTheFile_AndLeavesNothingElse()
    {
        // Arrange
        File.WriteAllText(_path, "old");

        // Act & Assert
        Assert.Throws<IOException>(() => AtomicFile.Write(_path, temporaryPath =>
        {
            File.WriteAllText(temporaryPath, "ne");

            throw new IOException("Disk full");
        }));
        Assert.Equal("old", File.ReadAllText(_path));
        Assert.Single(Directory.GetFiles(_folder));
    }

    [Fact]
    public void Write_WithoutOverwrite_KeepsAFileThatsThere()
    {
        // Arrange
        File.WriteAllText(_path, "old");

        // Act & Assert
        Assert.Throws<IOException>(() =>
            AtomicFile.Write(_path, temporaryPath => File.WriteAllText(temporaryPath, "new"), overwrite: false));
        Assert.Equal("old", File.ReadAllText(_path));
        Assert.Single(Directory.GetFiles(_folder));
    }
}
