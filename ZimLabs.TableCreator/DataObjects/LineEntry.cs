using System.Collections.Generic;

namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Represents a single entry of the given list
/// </summary>
internal sealed class LineEntry
{
    /// <summary>
    /// Gets or sets the id of the entry
    /// </summary>
    public int Id { get; }

    /// <summary>
    /// Gets or sets the value which indicates if the entry represents the header 
    /// </summary>
    public bool IsHeader { get; }

    /// <summary>
    /// Gets or sets the list with the values
    /// </summary>
    public List<ValueEntry> Values { get; set; } = new List<ValueEntry>();

    /// <summary>
    /// Creates a new instance of the <see cref="LineEntry"/>
    /// </summary>
    /// <param name="id">The id of the entry</param>
    /// <param name="isHeader">The value which indicates if the line represents the header (optional)</param>
    public LineEntry(int id, bool isHeader = false)
    {
        Id = id;
        IsHeader = isHeader;
    }
}