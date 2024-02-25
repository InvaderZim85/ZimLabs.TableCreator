using System.Text;

namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Provides the options for the list creation
/// </summary>
public sealed class TableCreatorListOptions
{
    /// <summary>
    /// Gets or sets the desired list type (default = <see cref="ListType.Bullets"/>)
    /// </summary>
    public ListType ListType { get; set; } = ListType.Bullets;

    /// <summary>
    /// Gets or sets the value which indicates if the properties should be aligned (default = <see langword="false"/>)
    /// <para />
    /// <see langword="true"/> to add dots to the end of the properties so that all properties have the same length
    /// </summary>
    public bool AlignProperties { get; set; }

    /// <summary>
    /// Gets or sets the desired encoding (default = <see cref="Encoding.UTF8"/>)
    /// <para />
    /// This value is only used in the file export
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Gets or sets the list with the override entries.
    /// <para />
    /// <b>Note</b>: If you add an entry, the original <see cref="AppearanceAttribute"/> of the desired property will be ignored
    /// </summary>
    public List<OverrideAttributeEntry> OverrideList { get; set; } = [];
}