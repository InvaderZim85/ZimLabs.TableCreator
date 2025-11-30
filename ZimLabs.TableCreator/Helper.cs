using System.Collections;
using System.Data;
using System.Text;
using ZimLabs.TableCreator.DataObjects;

namespace ZimLabs.TableCreator;

/// <summary>
/// Provides several helper functions
/// </summary>
internal static class Helper
{
    /// <summary>
    /// Creates the table
    /// </summary>
    /// <param name="outputType">The desired output type</param>
    /// <param name="printLineNumbers">The value which indicates if line numbers should be added</param>
    /// <param name="maxLineLength">The max. line length</param>
    /// <param name="widthList">The list with the column width entries</param>
    /// <param name="printList">The list with the value (which should be "printed")</param>
    /// <returns>The created table</returns>
    public static string CreateTable(OutputType outputType, bool printLineNumbers, int maxLineLength,
        List<ColumnWidth> widthList, List<LineEntry> printList)
    {
        // Create the result
        var result = new StringBuilder();

        // Print the first line (if the default type is chosen)
        if (outputType == OutputType.Default)
            result.AppendLine(PrintLine(outputType, printLineNumbers, maxLineLength, widthList));

        // Print the column header
        var header = printList.FirstOrDefault(f => f.IsHeader);
        result.AppendLine(header != null
            ? PrintLine(printLineNumbers, maxLineLength, widthList, header, true)
            : PrintHeaderLine(widthList));

        // Print the separator line (if the default type is chosen)
        result.AppendLine(PrintLine(outputType, printLineNumbers, maxLineLength, widthList));

        // Print the values
        foreach (var line in printList.Where(w => !w.IsHeader).OrderBy(o => o.Id))
        {
            result.AppendLine(PrintLine(printLineNumbers, maxLineLength, widthList, line, false));
        }

        // Print the footer (if the default type is chosen)
        if (outputType == OutputType.Default)
            result.AppendLine(PrintLine(outputType, printLineNumbers, maxLineLength, widthList));

        return result.ToString();
    }

    /// <summary>
    /// Creates a CSV formatted string of the value list.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="list">The list with the values.</param>
    /// <param name="delimiter">The delimiter which should be used for CSV.</param>
    /// <param name="printLineNumbers"><see langword="true"/> to print line numbers, otherwise <see langword="false"/>.</param>
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>.</param>
    /// <param name="encapsulateText">The value which indicates whether text values should be encapsulated with quotation marks.</param>
    /// <param name="overrideList">The list with the override entries.</param>
    /// <returns>The csv file content.</returns>
    public static string CreateCsv<T>(IEnumerable<T> list, string delimiter, bool printLineNumbers, bool addHeader, bool encapsulateText,
        List<OverrideAttributeEntry>? overrideList)
    {
        return CreateCsv(list, new TableCreatorOptions
        {
            Delimiter = delimiter,
            PrintLineNumbers = printLineNumbers,
            AddHeader = addHeader,
            EncapsulateText = encapsulateText,
            OverrideList = overrideList ?? []
        });
    }

    /// <summary>
    /// Creates a CSV formatted string of the value list.
    /// </summary>
    /// <typeparam name="T">The type of the data.</typeparam>
    /// <param name="list">The list which contains the values.</param>
    /// <param name="options">The desired options like the delimiter.</param>
    /// <returns>The CSV file content.</returns>
    public static string CreateCsv<T>(IEnumerable<T> list, TableCreatorOptions options)
    {
        // Remove all possible "null" values
        var tmpList = list.Where(w => w != null).ToList();

        if (tmpList.Count == 0)
            return string.Empty; // Return an empty string.

        // Prepare the result
        var content = new StringBuilder();

        // Get the properties
        var properties = GetProperties<T>(options.OverrideList);

        // Add the header (if desired)
        if (options.AddHeader)
        {
            var headerList = new List<string>();
            if (options.PrintLineNumbers)
                headerList.Add("Row");

            // Add the header
            headerList.AddRange(properties.Select(s => s.CustomName));

            // Add the header to the content
            content.AppendLine(string.Join(options.Delimiter, headerList));
        }

        // Add the content
        var rowCount = 1;
        foreach (var values in tmpList.Where(w => w != null).Select(entry => (from property in properties
                     where !property.Appearance.Ignore
                     select GetPropertyValue(entry!, property, true, options.EncapsulateText)).ToList()))
        {
            if (options.PrintLineNumbers)
                values.Insert(0, rowCount.ToString());

            content.AppendLine(string.Join(options.Delimiter, values));

            rowCount++;
        }

        // Return the created content.
        return content.ToString();
    }

    /// <summary>
    /// Creates a CSV formatted string of the list
    /// </summary>
    /// <param name="table">The data table</param>
    /// <param name="options">The desired options like the delimiter.</param>
    /// <returns>The csv file content</returns>
    public static string CreateCsv(DataTable table, TableCreatorOptions options)
    {
        if (table.Rows.Count == 0)
            return string.Empty;

        var content = new StringBuilder();

        // Get the properties
        var properties = GetProperties(table, options.OverrideList);

        // Add the header line
        if (options.AddHeader)
        {
            var headerList = new List<string>();
            if (options.PrintLineNumbers)
                headerList.Add("Row");

            headerList.AddRange(properties.Select(s => string.IsNullOrEmpty(s.Appearance.Name)
                ? s.Name
                : s.Appearance.Name));

            // Add the header to the content
            content.AppendLine(string.Join(options.Delimiter, headerList));
        }

        // Add the content
        var rowCount = 1;
        foreach (DataRow row in table.Rows)
        {
            var valueList = new List<string>();

            if (options.PrintLineNumbers)
                valueList.Add(rowCount.ToString());

            valueList.AddRange(properties.Select(property => GetPropertyValue(row, property.Name,
                property.Appearance.Format, property.Appearance.EncapsulateContent)));

            content.AppendLine(string.Join(options.Delimiter, valueList));

            rowCount++;
        }

        return content.ToString();
    }

    /// <summary>
    /// Prints a single line
    /// </summary>
    /// <param name="outputType">The desired output type</param>
    /// <param name="printLineNumbers">The value which indicates if line numbers should be printed</param>
    /// <param name="maxLineLength">The max. "line" length</param>
    /// <param name="widthList">The list with the column width</param>
    /// <param name="spacer">The default spacer</param>
    /// <returns>The line</returns>
    private static string PrintLine(OutputType outputType, bool printLineNumbers, int maxLineLength,
        List<ColumnWidth> widthList, int spacer = 2)
    {
        var lineStartEnd = outputType == OutputType.Markdown ? "|" : "+";

        var result = lineStartEnd;

        if (printLineNumbers)
        {
            result += outputType == OutputType.Markdown
                ? $"{"-:".PadLeft(maxLineLength + spacer, '-')}|"
                : $"{"-".PadRight(maxLineLength + spacer, '-')}+";
        }

        for (var i = 0; i < widthList.Count; i++)
        {
            var entry = widthList[i];
            var separator = i + 1 == widthList.Count ? lineStartEnd : "+";

            result += outputType == OutputType.Markdown
                ? entry.Align switch
                {
                    TextAlign.Left => $"{":-".PadRight(entry.Width + spacer, '-')}|",
                    TextAlign.Right => $"{"-:".PadLeft(entry.Width + spacer, '-')}|",
                    TextAlign.Center => $":{"-".PadRight(entry.Width, '-')}:|",
                    _ => $"{"-".PadRight(entry.Width + spacer, '-')}|"
                }
                : $"{"-".PadRight(entry.Width + spacer, '-')}{separator}";
        }

        return result;
    }

    /// <summary>
    /// Prints the header line
    /// </summary>
    /// <param name="widthList">The list with the column width</param>
    /// <returns>The header line</returns>
    private static string PrintHeaderLine(IEnumerable<ColumnWidth> widthList)
    {
        const string lineStartEnd = "|";

        return widthList.Aggregate(lineStartEnd,
            (current, entry) => current + $" {entry.ColumnName.PadRight(entry.Width)} {lineStartEnd}");
    }

    /// <summary>
    /// Prints a single line
    /// </summary>
    /// <param name="printLineNumbers">The value which indicates if line numbers should be printed</param>
    /// <param name="maxLineLength">The max. "line" length</param>
    /// <param name="widthList">The list with the column width</param>
    /// <param name="line">The line which should be printed</param>
    /// <param name="header">true when the header should be printed</param>
    /// <returns>The value line</returns>
    private static string PrintLine(bool printLineNumbers, int maxLineLength, IEnumerable<ColumnWidth> widthList, LineEntry line, bool header)
    {
        const string lineStartEnd = "|";

        var result = lineStartEnd;

        if (printLineNumbers)
        {
            result += header
                ? $" {"Row".PadRight(maxLineLength)} {lineStartEnd}"
                : $" {line.Id.ToString().PadLeft(maxLineLength)} {lineStartEnd}";
        }

        foreach (var entry in widthList)
        {
            var value = line.Values.FirstOrDefault(f => f.ColumnName.Equals(entry.ColumnName));
            if (value == null)
                continue;

            result += header
                ? $" {value.DisplayName.PadRight(entry.Width)} {lineStartEnd}"
                : entry.Align == TextAlign.Left
                    ? $" {value.Value.PadRight(entry.Width)} {lineStartEnd}"
                    : $" {value.Value.PadLeft(entry.Width)} {lineStartEnd}";
        }

        return result;
    }

    /// <summary>
    /// Gets the max. length for every column
    /// </summary>
    /// <param name="properties">The list with the properties</param>
    /// <param name="printList">The list with the print entries</param>
    /// <returns>The list with the max. length</returns>
    public static List<ColumnWidth> GetColumnWidthList(IEnumerable<Property> properties,
        IReadOnlyCollection<LineEntry> printList)
    {
        return
        [
            .. from property in properties
            let maxValue = printList.SelectMany(s => s.Values)
                .Where(w => w.ColumnName.Equals(property.Name))
                .Max(m => m.Value.Length)
            select new ColumnWidth(property.Name, maxValue, property.Appearance.TextAlign)
        ];
    }

    /// <summary>
    /// Gets the value of the property
    /// </summary>
    /// <param name="obj">The object which contains the data</param>
    /// <param name="property">The property with the needed values (name, appearance values)</param>
    /// <param name="isCsvExport"><see langword="true"/> to indicate that the method is used during the CSV export. If so, the <see cref="Property.EncapsulateContent"/> value should be considered</param>
    /// <param name="encapsulateText"><see langword="true"/> to encapsulate text values even if the property value is set to <see langword="false"/>.</param>
    /// <returns>The property value</returns>
    public static string GetPropertyValue(object obj, Property property, bool isCsvExport, bool encapsulateText)
    {
        var tmpProperty = obj.GetType().GetProperty(property.Name);
        var value = tmpProperty?.GetValue(obj, null);

        if (value == null)
            return string.Empty;

        var tmpValue = value.ToString() ?? string.Empty;
        var formattedValue = string.IsNullOrEmpty(property.Appearance.Format)
            ? tmpValue
            : string.Format($"{{0:{property.Appearance.Format}}}", value);

        if (!isCsvExport)
            return formattedValue;

        // Note: We ignore the value "EncapsulateContent" if the value "encapsulateText" is set to true.
        //       But note that the value only applies to text fields! All other types are not taken into account.
        var isText = tmpProperty != null && tmpProperty.PropertyType == typeof(string);
        if (isText && encapsulateText)
            return $"\"{formattedValue}\"";

        return property.EncapsulateContent ? $"\"{formattedValue}\"" : formattedValue;
    }

    /// <summary>
    /// Gets the value of the property
    /// </summary>
    /// <param name="row">The data row</param>
    /// <param name="propertyName">The name of the property aka column name</param>
    /// <param name="format">The desired format</param>
    /// <param name="encapsulateContent">The value which indicates if the content should be encapsulated by quotes</param>
    /// <returns>The property value</returns>
    public static string GetPropertyValue(DataRow row, string propertyName, string format, bool encapsulateContent)
    {
        var value = row[propertyName]; // Note: Cannot be null, because the name of the property was gathered by the GetProperties method
        var tmpValue = value.ToString() ?? string.Empty;

        var formattedValue = string.IsNullOrEmpty(format) ? tmpValue : string.Format($"{{0:{format}}}", value);

        return encapsulateContent ? $"\"{formattedValue}\"" : formattedValue;
    }

    /// <summary>
    /// Gets all properties of the specified type
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The list with the properties</returns>
    public static IReadOnlyCollection<Property> GetProperties<T>(List<OverrideAttributeEntry>? overrideList)
    {
        var properties = typeof(T).GetProperties();
        var values = properties.Select(s => (Property)s).ToList();

        if (overrideList == null || overrideList.Count == 0)
            return [.. values.Where(w => !w.Ignore).OrderBy(o => o.Order)];

        foreach (var entry in values)
        {
            var overrideEntry = overrideList.FirstOrDefault(f => f.PropertyName.Equals(entry.Name, StringComparison.OrdinalIgnoreCase));
            if (overrideEntry != null)
                entry.Appearance = overrideEntry.Appearance;
        }

        return [.. values.Where(w => !w.Ignore).OrderBy(o => o.Order)];
    }

    /// <summary>
    /// Gets all properties of the specific data table
    /// </summary>
    /// <param name="table">The data table</param>
    /// <param name="overrideList">The override list</param>
    /// <returns>The list with the properties</returns>
    public static IReadOnlyCollection<Property> GetProperties(DataTable table, List<OverrideAttributeEntry>? overrideList)
    {
        var values = (from DataColumn column in table.Columns
            select new Property(column.ColumnName,
                overrideList
                    ?.FirstOrDefault(f => f.PropertyName.Equals(column.ColumnName, StringComparison.OrdinalIgnoreCase))
                    ?.Appearance)).ToList();

        return [.. values.Where(w => !w.Ignore).OrderBy(o => o.Order)];
    }

    /// <summary>
    /// Checks if the specified type is supported and throws a <see cref="NotSupportedException"/> exception if not.
    /// </summary>
    /// <typeparam name="T">The desired type.</typeparam>
    /// <exception cref="NotSupportedException">Will be thrown if the type is not supported.</exception>
    public static void ThrowIfNotSupported<T>()
    {
        var type = typeof(T);
        if (typeof(IEnumerable).IsAssignableFrom(type))
            throw new NotSupportedException("The specified type is not supported by this method. Please choose \"CreateTable\" or \"SaveTable\" instead.");
    }
}