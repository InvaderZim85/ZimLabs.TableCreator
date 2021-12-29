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
        /// Gets or sets the text align (will be ignored when the output type is <see cref="OutputType.Csv"/>)
        /// </summary>
        public TextAlign TextAlign { get; set; } = TextAlign.Left;

        /// <summary>
        /// Gets or sets the value which indicates if the property should be ignored. If <see langword="true"/> the other settings will be ignored.
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        /// Gets or sets the order of the property.
        /// <para/>
        /// NOTE: Properties that do not have an order value are sorted by the position in the class.
        /// </summary>
        public int Order { get; set; } = -1;
    }
}
