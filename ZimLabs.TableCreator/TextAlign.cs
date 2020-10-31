namespace ZimLabs.TableCreator
{
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
}
