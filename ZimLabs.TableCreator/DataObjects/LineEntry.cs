namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Represents a single entry of the given list
/// </summary>
/// <remarks>
/// Creates a new instance of the <see cref="LineEntry"/>
/// </remarks>
/// <param name="id">The id of the entry</param>
/// <param name="isHeader">The value which indicates if the line represents the header (optional)</param>
internal sealed class LineEntry(int id, bool isHeader = false)
{
    /// <summary>
    /// Gets or sets the id of the entry
    /// </summary>
    public int Id { get; } = id;

    /// <summary>
    /// Gets or sets the value which indicates if the entry represents the header 
    /// </summary>
    public bool IsHeader { get; } = isHeader;

    /// <summary>
    /// Gets or sets the list with the values
    /// </summary>
    public List<ValueEntry> Values { get; set; } = [];
}