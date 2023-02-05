namespace ZimLabs.TableCreator.DataObjects;

/// <summary>
/// Provides the maximal length of a column
/// </summary>
internal class ColumnWidth
{
    /// <summary>
    /// Gets the name of the column
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Gets the width of the column
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the align of the text
    /// </summary>
    public TextAlign Align { get; }

    /// <summary>
    /// Creates a new instance of the <see cref="ColumnWidth"/>
    /// </summary>
    /// <param name="columnName">The name of the column</param>
    /// <param name="width">The width of the column</param>
    /// <param name="align">The align of the column (optional)</param>
    public ColumnWidth(string columnName, int width, TextAlign align = TextAlign.Left)
    {
        ColumnName = columnName;
        Width = width;
        Align = align;
    }
}