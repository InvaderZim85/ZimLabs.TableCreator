namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Represents a single value
/// </summary>
internal sealed class ValueEntry
{
    /// <summary>
    /// Contains the custom column name
    /// </summary>
    private readonly string _customColumnName;

    /// <summary>
    /// Gets the name of the column
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Gets the display name of the column
    /// </summary>
    public string DisplayName => string.IsNullOrEmpty(_customColumnName) ? ColumnName : _customColumnName;

    /// <summary>
    /// Gets the value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ValueEntry"/>
    /// </summary>
    /// <param name="columnName">The name of the column</param>
    /// <param name="value">The value which should be shown</param>
    /// <param name="customColumnName">THe display name</param>
    public ValueEntry(string columnName, string value, string customColumnName = "")
    {
        ColumnName = columnName;
        Value = value;
        _customColumnName = customColumnName;
    }
}