namespace ZimLabs.TableCreator;

/// <summary>
/// The different supported table types
/// </summary>
public enum OutputType
{
    /// <summary>
    /// The default table type
    /// </summary>
    Default,

    /// <summary>
    /// The table as markdown
    /// </summary>
    Markdown,

    /// <summary>
    /// The table as a comma separated value list
    /// </summary>
    Csv
}

/// <summary>
/// The different list types
/// </summary>
public enum ListType
{
    /// <summary>
    /// Bullet list
    /// </summary>
    Bullets,

    /// <summary>
    /// Numbered list
    /// </summary>
    Numbers
}

/// <summary>
/// The different align types
/// </summary>
public enum TextAlign
{
    /// <summary>
    /// Aligns the text to the left
    /// </summary>
    Left,

    /// <summary>
    /// Aligns the text to the right
    /// </summary>
    Right,

    /// <summary>
    /// Aligns the text to the center (only available when output type is set to <see cref="OutputType.Markdown"/>)
    /// </summary>
    Center
}