using System.Text;

namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Provides the options for the list creation
/// </summary>
public sealed class TableCreatorListOptions
{
    /// <summary>
    /// Gets or sets the desired list type (default = <see cref="ListType.Bullets"/>).
    /// </summary>
    public ListType ListType { get; init; } = ListType.Bullets;

    /// <summary>
    /// Gets or sets the value which indicates if the properties should be aligned (default = <see langword="false"/>).
    /// </summary>
    /// <remarks>
    /// <see langword="true"/> to add dots to the end of the properties so that all properties have the same length.
    /// </remarks>
    public bool AlignProperties { get; init; }

    /// <summary>
    /// Gets or sets the desired encoding (default = <see cref="Encoding.UTF8"/>).
    /// </summary>
    /// <remarks>
    /// This value is only used in the file export.
    /// </remarks>
    public Encoding Encoding { get; init; } = Encoding.UTF8;

    /// <summary>
    /// Gets or sets the list with the override entries.
    /// </summary>
    /// <remarks>
    /// <b>Note</b>: If you add an entry, the original <see cref="AppearanceAttribute"/> of the desired property will be ignored.
    /// </remarks>
    public List<OverrideAttributeEntry> OverrideList { get; init; } = [];

    /// <summary>
    /// Creates a new, default instance of the options with its default settings (<see cref="ListType"/> = <see cref="ListType.Bullets"/>, <see cref="Encoding"/> = <see cref="Encoding.UTF8"/>).
    /// </summary>
    public TableCreatorListOptions()
    {
        
    }
}