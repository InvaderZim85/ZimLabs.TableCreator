namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Provides the maximal length of a column
/// </summary>
/// <remarks>
/// Creates a new instance of the <see cref="ColumnWidth"/>
/// </remarks>
/// <param name="columnName">The name of the column</param>
/// <param name="width">The width of the column</param>
/// <param name="align">The alignment of the column (optional)</param>
internal readonly struct ColumnWidth(string columnName, int width, TextAlign align = TextAlign.Left)
{
    /// <summary>
    /// Gets the name of the column
    /// </summary>
    public string ColumnName { get; } = columnName;

    /// <summary>
    /// Gets the width of the column
    /// </summary>
    public int Width { get; } = width;

    /// <summary>
    /// Gets the alignment of the text
    /// </summary>
    public TextAlign Align { get; } = align;
}