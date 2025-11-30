using System.Text;

namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Provides the options for the table creation.
/// </summary>
public sealed class TableCreatorOptions
{
    /// <summary>
    /// Gets or sets the desired output type (default = <see cref="OutputType.Default"/>).
    /// </summary>
    public OutputType OutputType { get; init; } = OutputType.Default;

    /// <summary>
    /// Gets or sets the value which indicates if a "line number" should be added to the list (default = <see langword="false"/>).
    /// </summary>
    public bool PrintLineNumbers { get; init; }

    /// <summary>
    /// Gets or sets the delimiter which should be used for the CSV export (default = <c>;</c>).
    /// </summary>
    /// <remarks>
    /// <b>Note</b>: This value is only needed when the <see cref="OutputType"/> is set to <see cref="OutputType.Csv"/>.
    /// </remarks>
    public string Delimiter { get; init; } = ";";

    /// <summary>
    /// Gets or sets the desired encoding (default = <see cref="Encoding.UTF8"/>).
    /// </summary>
    /// <remarks>
    /// This value is only used in the file export.
    /// </remarks>
    public Encoding Encoding { get; init; } = Encoding.UTF8;

    /// <summary>
    /// Gets or sets the value which indicates if a header should be added to the CSV content (default = <see langword="true"/>).
    /// </summary>
    /// <remarks>
    /// <b>Note</b>: This options is only needed for the CSV Export.
    /// </remarks>
    public bool AddHeader { get; init; } = true;

    /// <summary>
    /// Gets or sets the value which indicates whether <i>text</i> values should be encapsulated with quotation marks (default = <see langword="false"/>).
    /// </summary>
    /// <remarks>
    /// <b>Note</b>: If this value is set to <see langword="true"/>, the attribute is ignored on the respective property for <i>text</i> fields.
    /// </remarks>
    public bool EncapsulateText { get; init; }

    /// <summary>
    /// Gets or sets the list with the override entries.
    /// </summary>
    /// <remarks>
    /// <b>Note</b>: If you add an entry, the original <see cref="AppearanceAttribute"/> of the desired property will be ignored.
    /// </remarks>
    public List<OverrideAttributeEntry> OverrideList { get; init; } = [];

    /// <summary>
    /// Creates a new, empty instance of the options with its default settings (<see cref="OutputType"/> = <see cref="OutputType.Default"/>, <see cref="Delimiter"/> = <c>;</c> - only for CSV, <see cref="Encoding"/> = <see cref="Encoding.UTF8"/>).
    /// </summary>
    public TableCreatorOptions() { }

    /// <summary>
    /// Creates a new instance with the specified information and the default settings <see cref="Delimiter"/> = <c>;</c> (only for CSV) and <see cref="Encoding"/> = <see cref="Encoding.UTF8"/>.
    /// </summary>
    /// <param name="outputType">The output type.</param>
    public TableCreatorOptions(OutputType outputType)
    {
        OutputType = outputType;
    }
}