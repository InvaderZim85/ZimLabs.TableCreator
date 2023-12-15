using System.Diagnostics;
using System.Reflection;

namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Represents the information about a property
/// </summary>
/// <remarks>
/// Creates a new instance of the <see cref="Property"/>
/// </remarks>
/// <param name="name">The name of the property</param>
/// <param name="appearance">The appearance value</param>
[DebuggerDisplay("{Name}")]
internal sealed class Property(string name, AppearanceAttribute? appearance)
{
    /// <summary>
    /// Gets the name of the property
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// Gets the custom name if available, otherwise the original name will be returned
    /// </summary>
    public string CustomName => string.IsNullOrEmpty(Appearance.Name) ? Name : Appearance.Name;

    /// <summary>
    /// Gets or sets the appearance
    /// </summary>
    public AppearanceAttribute Appearance { get; set; } = appearance ?? new AppearanceAttribute();

    /// <summary>
    /// Gets the value which indicates if the property should be ignored
    /// </summary>
    public bool Ignore => Appearance.Ignore;

    /// <summary>
    /// Gets the order value of the property
    /// </summary>
    public int Order => Appearance.Order;

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> into a <see cref="Property"/>
    /// </summary>
    /// <param name="prop">The original property</param>
    public static explicit operator Property(PropertyInfo prop)
    {
        var attribute = prop.GetCustomAttribute<AppearanceAttribute>();
        return new Property(prop.Name, attribute);
    }
}