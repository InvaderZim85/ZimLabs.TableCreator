namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Represents a key value entry
/// </summary>
/// <param name="key">The key</param>
/// <param name="value">The value of the key</param>
internal sealed class KeyValueEntry(string key, string value)
{
    /// <summary>
    /// Gets the key
    /// </summary>
    public string Key { get; } = key;

    /// <summary>
    /// Gets the value
    /// </summary>
    public string Value { get; } = value;
}