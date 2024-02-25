namespace ZimLabs.TableCreator;

/// <summary>
/// Represents an attribute to modify the appearance of a column
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public sealed class AppearanceAttribute : Attribute
{
    /// <summary>
    /// Gets or sets the name which should be used for the <i>column</i>
    /// <para/>
    /// With this property you can overwrite the original property name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the format of the value (default = <see cref="string.Empty"/>)
    /// <para />
    /// For example: <c>yyyy-MM-dd HH:mm:ss</c> will produce <c>2023-12-15 20:15:00</c> if the property is a <see cref="DateTime"/>
    /// <para />
    /// For more information about the format see <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">MSDN - formatting types</a>
    /// </summary>
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text align (will be ignored when the output type is <see cref="OutputType.Csv"/>) (default = <see cref="TextAlign.Left"/>)
    /// </summary>
    public TextAlign TextAlign { get; set; } = TextAlign.Left;

    /// <summary>
    /// Gets or sets the value which indicates if the property should be ignored. If <see langword="true"/> the other settings will be ignored (default = <see langword="false"/>)
    /// </summary>
    public bool Ignore { get; set; }

    /// <summary>
    /// Gets or sets the order of the property (default = <c>-1</c>).
    /// <para/>
    /// <b>NOTE</b>: Properties that do not have an order value are sorted by the position in the class.
    /// </summary>
    public int Order { get; set; } = -1;

    /// <summary>
    /// Gets or sets the value which indicates if the content should be encapsulated by quotes (default = <see langword="false"/>)
    /// <para />
    /// <b>Example</b>: <c>...;"Some content";...</c>
    /// </summary>
    public bool EncapsulateContent { get; set; }

    /// <summary>
    /// Creates a new empty instance with the default values
    /// </summary>
    public AppearanceAttribute() { }

    /// <summary>
    /// Creates a new instance and sets the ignore value
    /// </summary>
    /// <param name="ignore"><see langword="true"/> to ignore the property, otherwise <see langword="false"/></param>
    public AppearanceAttribute(bool ignore)
    {
        Ignore = ignore;
    }

    /// <summary>
    /// Creates a new instance and sets the main values
    /// </summary>
    /// <param name="name">The name which should be used for the <i>column</i></param>
    /// <param name="format">
    /// The desired format (for example <c>yyyy-MM-dd HH:mm:ss</c> will produce <c>2023-12-15 20:15:00</c> if the property is a <see cref="DateTime"/>
    /// <para />
    /// For more information about the format see <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/formatting-types">MSDN - formatting types</a>
    /// </param> 
    /// <param name="textAlign">The desired text alignment (will be ignored when the output type is <see cref="OutputType.Csv"/>)</param>
    /// <param name="order">
    /// The desired order of the property.
    /// <para />
    /// <b>NOTE</b>: Properties that do not have an order value are sorted by the position in the class
    /// </param>
    /// <param name="encapsulateContent">
    /// <see langword="true"/> to encapsulate the content of the property with quotes.
    /// <para />
    /// <b>Example</b>: <c>...;"Some content";...</c>
    /// </param>
    public AppearanceAttribute(string name = "", string format = "", TextAlign textAlign = TextAlign.Left, int order = -1, bool encapsulateContent = false)
    {
        Name = name;
        Format = format;
        TextAlign = textAlign;
        Ignore = false;
        Order = order;
        EncapsulateContent = encapsulateContent;
    }
}