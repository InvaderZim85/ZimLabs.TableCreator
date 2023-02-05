namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Represents an entry which provides the possibility to override an existing property <see cref="AppearanceAttribute"/>
/// </summary>
public sealed class OverrideAttributeEntry
{
    /// <summary>
    /// Gets or sets the name of the property
    /// </summary>
    public string PropertyName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the new attribute
    /// </summary>
    public AppearanceAttribute Appearance { get; set; }

    /// <summary>
    /// Creates a new empty instance of the <see cref="OverrideAttributeEntry"/>
    /// </summary>
    public OverrideAttributeEntry() { }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="appearance">The new appearance attribute</param>
    public OverrideAttributeEntry(string propertyName, AppearanceAttribute appearance)
    {
        PropertyName = propertyName;
        Appearance = appearance;
    }
}