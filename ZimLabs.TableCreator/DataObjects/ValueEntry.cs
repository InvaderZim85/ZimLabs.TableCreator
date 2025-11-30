namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Represents a single value
/// </summary>
internal sealed class ValueEntry
{
    /// <summary>
    /// Gets the name of the column
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Gets the display name of the column
    /// </summary>
    public string DisplayName => string.IsNullOrEmpty(field) ? ColumnName : field;

    /// <summary>
    /// Gets the value
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ValueEntry"/>
    /// </summary>
    /// <param name="columnName">The name of the column</param>
    /// <param name="value">The value which should be shown</param>
    /// <param name="customColumnName">The display name</param>
    public ValueEntry(string columnName, string value, string customColumnName = "")
    {
        ColumnName = columnName;
        Value = value;
        DisplayName = customColumnName;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="ValueEntry"/>
    /// </summary>
    /// <param name="property">The property</param>
    public ValueEntry(Property property)
    {
        ColumnName = property.Name;
        Value = string.IsNullOrEmpty(property.Appearance.Name) ? property.Name : property.Appearance.Name;
        DisplayName = property.Appearance.Name;
    }
}