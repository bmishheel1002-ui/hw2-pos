using System.Text.Json;

namespace Pos.Infrastructure;

/// <summary>
/// Reads and writes simple JSON files for the infrastructure adapters.
/// </summary>
internal sealed class JsonFileStore<T>(string filePath)
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public List<T> Load()
    {
        if (!File.Exists(filePath))
            return [];

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<T>>(json, Options)
            ?? throw new InvalidDataException($"The data file '{filePath}' is empty or invalid.");
    }

    public void Save(List<T> items)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var json = JsonSerializer.Serialize(items, Options);
        File.WriteAllText(filePath, json);
    }
}
