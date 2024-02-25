using System.Text;

namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Provides the options for the table creation
/// </summary>
public sealed class TableCreatorOptions
{
    /// <summary>
    /// Gets or sets the desired output type (default = <see cref="OutputType.Default"/>)
    /// </summary>
    public OutputType OutputType { get; set; } = OutputType.Default;

    /// <summary>
    /// Gets or sets the value which indicates if a "line number" should be added to the list (default = <see langword="false"/>)
    /// </summary>
    public bool PrintLineNumbers { get; set; } = false;

    /// <summary>
    /// Gets or sets the delimiter which should be used for the CSV export. (default = <c>;</c>)
    /// <para />
    /// <b>Note</b>: This value is only needed when the <see cref="OutputType"/> is set to <see cref="OutputType.Csv"/>
    /// </summary>
    public string Delimiter { get; set; } = ";";

    /// <summary>
    /// Gets or sets the desired encoding (default = <see cref="Encoding.UTF8"/>)
    /// <para />
    /// This value is only used in the file export
    /// </summary>
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    /// <summary>
    /// Gets or sets the value which indicates if a header should be added to the CSV content (default = <see langword="true"/>)
    /// <para />
    /// <b>Note</b>: This options is only needed for the CSV Export
    /// </summary>
    public bool AddHeader { get; set; } = true;

    /// <summary>
    /// Gets or sets the list with the override entries.
    /// <para />
    /// <b>Note</b>: If you add an entry, the original <see cref="AppearanceAttribute"/> of the desired property will be ignored
    /// </summary>
    public List<OverrideAttributeEntry> OverrideList { get; set; } = [];
}