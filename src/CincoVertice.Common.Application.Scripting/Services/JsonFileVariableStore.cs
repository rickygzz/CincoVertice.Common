using System.Diagnostics.CodeAnalysis;
using System.Text.Encodings.Web;
using System.Text.Json;
using CincoVertice.Common.Application.Scripting.Interfaces;
using CincoVertice.Common.Utils.Helpers;

namespace CincoVertice.Common.Application.Scripting.Services;

/// <summary>
///     Global variables in a JSON file, e.g. { "count": 3, "name": "Ricky", "sites": ["a.com", "b.com"] }. The file
///     is read on every call and written on every change, so all scripts share it and edits by hand apply at once.
///     It may have // comments and trailing commas; they are dropped on the next change.
/// </summary>
public sealed class JsonFileVariableStore : IScriptVariableStore
{
    private static readonly JsonDocumentOptions _readOptions = new()
    {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    // Indented, with accents and emojis written as they are, so the file is easy to read and edit
    private static readonly JsonWriterOptions _writeOptions = new()
    {
        Indented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    public JsonFileVariableStore(string filePath)
    {
        FilePath = Path.GetFullPath(filePath);
    }

    public string FilePath { get; }

    /// <exception cref="InvalidOperationException">The file is not valid, or has a value scripts can't use.</exception>
    public bool TryGet(string name, [NotNullWhen(true)] out object? value) => Load().TryGetValue(name, out value);

    public void Set(string name, object value)
    {
        var variables = Load();

        // Removed first, so the name keeps the spelling of the last set
        variables.Remove(name);
        variables[name] = value;
        Save(variables);
    }

    public bool Remove(string name)
    {
        var variables = Load();

        if (!variables.Remove(name))
        {
            return false;
        }

        Save(variables);

        return true;
    }

    public IReadOnlyList<string> GetNames() => [.. Load().Keys.Order(StringComparer.OrdinalIgnoreCase)];

    private Dictionary<string, object> Load()
    {
        var variables = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        if (!File.Exists(FilePath))
        {
            return variables;
        }

        string json = File.ReadAllText(FilePath);

        if (string.IsNullOrWhiteSpace(json))
        {
            return variables;
        }

        try
        {
            using var document = JsonDocument.Parse(json, _readOptions);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw Invalid("it must hold { \"name\": value, ... }");
            }

            foreach (var property in document.RootElement.EnumerateObject())
            {
                variables[property.Name] = FromJson(property.Value, property.Name);
            }
        }
        catch (JsonException ex)
        {
            throw Invalid(ex.Message);
        }

        return variables;
    }

    private object FromJson(JsonElement element, string name) => element.ValueKind switch
    {
        JsonValueKind.Number when element.TryGetDecimal(out decimal number) => number,
        JsonValueKind.String => element.GetString()!,
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Array => element.EnumerateArray().Select(item => FromJson(item, name)).ToList().AsReadOnly(),
        _ => throw Invalid($"'{name}' must be a number, a text, true/false or a list, not {element.ValueKind}"),
    };

    /// <summary>
    ///     Writes a new file and then replaces the old one, so a failure never leaves half a file.
    /// </summary>
    private void Save(Dictionary<string, object> variables)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);

        AtomicFile.Write(FilePath, temporaryPath =>
        {
            using var stream = File.Create(temporaryPath);
            using var writer = new Utf8JsonWriter(stream, _writeOptions);

            writer.WriteStartObject();

            foreach (var (name, value) in variables.OrderBy(v => v.Key, StringComparer.OrdinalIgnoreCase))
            {
                writer.WritePropertyName(name);
                WriteValue(writer, value);
            }

            writer.WriteEndObject();
        });
    }

    private static void WriteValue(Utf8JsonWriter writer, object value)
    {
        switch (value)
        {
            case decimal number:
                writer.WriteNumberValue(number);
                break;

            case string text:
                writer.WriteStringValue(text);
                break;

            case bool flag:
                writer.WriteBooleanValue(flag);
                break;

            case IReadOnlyList<object> list:
                writer.WriteStartArray();

                foreach (var item in list)
                {
                    WriteValue(writer, item);
                }

                writer.WriteEndArray();
                break;

            default:
                throw new InvalidOperationException($"A {value.GetType().Name} can't be stored.");
        }
    }

    private InvalidOperationException Invalid(string reason) => new($"{FilePath} is not valid: {reason}");
}
