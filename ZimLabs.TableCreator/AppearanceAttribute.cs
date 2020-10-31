using System;

namespace ZimLabs.TableCreator
{
    /// <summary>
    /// Represents an attribute to modify the appearance of a column
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class AppearanceAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the column which should be used
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the format of the value
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the text align
        /// </summary>
        public TextAlign TextAlign { get; set; } = TextAlign.Left;
    }
}
