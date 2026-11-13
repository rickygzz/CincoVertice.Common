using CincoVertice.Common.Application.Scripting.Services;
using Xunit;

namespace CincoVertice.Common.Application.Scripting.Tests;

public class JsonFileVariableStoreTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "JsonFileVariableStoreTests_" + Guid.NewGuid());

    private string FilePath => Path.Combine(_folder, "config", "global.vars");

    public void Dispose()
    {
        if (Directory.Exists(_folder))
        {
            Directory.Delete(_folder, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public void Set_WritesValuesThatAnotherStoreReadsBack()
    {
        // Arrange
        object list = new List<object> { 1m, "a", new List<object> { true }.AsReadOnly() }.AsReadOnly();

        // Act
        var writer = new JsonFileVariableStore(FilePath);
        writer.Set("count", 3m);
        writer.Set("name", "Ñoño 📝 \"quoted\"\nsecond line");
        writer.Set("done", false);
        writer.Set("items", list);

        // Assert
        var reader = new JsonFileVariableStore(FilePath);
        Assert.Equal(["count", "done", "items", "name"], reader.GetNames());
        Assert.True(reader.TryGet("COUNT", out object? count));
        Assert.Equal(3m, count);
        Assert.True(reader.TryGet("name", out object? name));
        Assert.Equal("Ñoño 📝 \"quoted\"\nsecond line", name);
        Assert.True(reader.TryGet("done", out object? done));
        Assert.Equal(false, done);
        Assert.True(reader.TryGet("items", out object? items));
        Assert.True(ScriptValue.AreEqual(list, items));
    }

    [Fact]
    public void Set_WritesReadableJson()
    {
        // Act
        new JsonFileVariableStore(FilePath).Set("name", "Ñoño");

        // Assert
        Assert.Equal("{\n  \"name\": \"Ñoño\"\n}", File.ReadAllText(FilePath).ReplaceLineEndings("\n"));
    }

    [Fact]
    public void TryGet_ReadsFileEditedByHand()
    {
        // Arrange
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, "{\n  // my settings\n  \"site\": \"github.com\",\n  \"max\": 10,\n}");
        var store = new JsonFileVariableStore(FilePath);

        // Act
        bool found = store.TryGet("max", out object? max);

        // Assert
        Assert.True(found);
        Assert.Equal(10m, max);
        Assert.Equal(["max", "site"], store.GetNames());
    }

    [Fact]
    public void TryGet_WhenFileIsMissing_FindsNothing()
    {
        // Act
        var store = new JsonFileVariableStore(FilePath);

        // Assert
        Assert.False(store.TryGet("x", out _));
        Assert.Empty(store.GetNames());
        Assert.False(store.Remove("x"));
    }

    [Fact]
    public void Remove_DeletesTheVariableFromTheFile()
    {
        // Arrange
        var store = new JsonFileVariableStore(FilePath);
        store.Set("a", 1m);
        store.Set("b", 2m);

        // Act
        bool removed = store.Remove("A");

        // Assert
        Assert.True(removed);
        Assert.Equal(["b"], new JsonFileVariableStore(FilePath).GetNames());
    }

    [Theory]
    [InlineData("{ \"a\": ", "is not valid")]
    [InlineData("[1, 2]", "it must hold { \"name\": value, ... }")]
    [InlineData("{ \"a\": null }", "'a' must be a number, a text, true/false or a list, not Null")]
    [InlineData("{ \"a\": { \"b\": 1 } }", "'a' must be a number, a text, true/false or a list, not Object")]
    public void TryGet_WhenFileIsNotValid_SaysWhy(string json, string message)
    {
        // Arrange
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, json);

        var store = new JsonFileVariableStore(FilePath);

        // Act
        var error = Assert.Throws<InvalidOperationException>(() => store.TryGet("a", out _));

        // Assert
        Assert.Contains(FilePath, error.Message);
        Assert.Contains(message, error.Message);
    }
}
