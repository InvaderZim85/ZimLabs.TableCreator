using System.Collections;
using System.Data;
using System.Text;
using ZimLabs.TableCreator.DataObjects;

namespace ZimLabs.TableCreator;

/// <summary>
/// Provides several helper functions
/// </summary>
internal static class TableHelper
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
    /// Creates a CSV formatted string of the list
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <param name="list">The list with the values</param>
    /// <param name="delimiter">The delimiter which should be used for CSV</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false</param>
    /// <param name="overrideList">The list with the override entries</param>
    /// <returns>The csv file content</returns>
    public static string CreateCsv<T>(IEnumerable<T> list, string delimiter, bool printLineNumbers,
        List<OverrideAttributeEntry>? overrideList)
    {
        var tmpList = list.Where(w => w != null).ToList();

        if (!tmpList.Any())
            return string.Empty;

        var content = new StringBuilder();

        // Get the properties
        var properties = GetProperties<T>(overrideList);

        // Add header line
        var headerList = new List<string>();
        if (printLineNumbers)
            headerList.Add("Row");

        headerList.AddRange(properties.Select(s => string.IsNullOrEmpty(s.Appearance.Name)
            ? s.Name
            : s.Appearance.Name));

        content.AppendLine(string.Join(delimiter, headerList));

        // Add the content
        var rowCount = 1;
        foreach (var valueList in tmpList.Where(w => w != null).Select(entry => (from property in properties
                     where !property.Appearance.Ignore
                     select GetPropertyValue(entry!, property.Name, property.Appearance.Format)).ToList()))
        {
            if (printLineNumbers)
                valueList.Insert(0, rowCount.ToString());

            content.AppendLine(string.Join(delimiter, valueList));

            rowCount++;
        }

        // Return the result
        return content.ToString();
    }

    /// <summary>
    /// Creates a CSV formatted string of the list
    /// </summary>
    /// <param name="table">The data table</param>
    /// <param name="delimiter">The delimiter which should be used for CSV</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false</param>
    /// <param name="overrideList">The list with the override entries</param>
    /// <returns>The csv file content</returns>
    public static string CreateCsv(DataTable table, string delimiter, bool printLineNumbers,
        List<OverrideAttributeEntry>? overrideList)
    {
        if (table.Rows.Count == 0)
            return string.Empty;

        var content = new StringBuilder();

        // Get the properties
        var properties = GetProperties(table, overrideList);

        // Add the header line
        var headerList = new List<string>();
        if (printLineNumbers)
            headerList.Add("Row");

        headerList.AddRange(properties.Select(s => string.IsNullOrEmpty(s.Appearance.Name)
            ? s.Name
            : s.Appearance.Name));

        // Add the header to the content
        content.AppendLine(string.Join(delimiter, headerList));

        // Add the content
        var rowCount = 1;
        foreach (DataRow row in table.Rows)
        {
            var valueList = new List<string>();

            if (printLineNumbers)
                valueList.Add(rowCount.ToString());

            valueList.AddRange(properties.Select(property => row[property.Name].ToString() ?? string.Empty));

            content.AppendLine(string.Join(delimiter, valueList));

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
    public static string PrintLine(OutputType outputType, bool printLineNumbers, int maxLineLength,
        IReadOnlyList<ColumnWidth> widthList, int spacer = 2)
    {
        var lineStartEnd = "+";
        if (outputType == OutputType.Markdown)
            lineStartEnd = "|";

        var result = lineStartEnd;

        if (printLineNumbers)
        {
            if (outputType == OutputType.Markdown)
                result += $"{"-:".PadLeft(maxLineLength + spacer, '-')}|";
            else
                result += $"{"-".PadRight(maxLineLength + spacer, '-')}+";
        }


        for (var i = 0; i < widthList.Count; i++)
        {
            var entry = widthList[i];
            var separator = i + 1 == widthList.Count ? lineStartEnd : "+";
            if (outputType == OutputType.Markdown)
            {
                result += entry.Align switch
                {
                    TextAlign.Left => $"{":-".PadRight(entry.Width + spacer, '-')}|",
                    TextAlign.Right => $"{"-:".PadLeft(entry.Width + spacer, '-')}|",
                    TextAlign.Center => $":{"-".PadRight(entry.Width, '-')}:|",
                    _ => $"{"-".PadRight(entry.Width + spacer, '-')}|"
                };
            }
            else
            {
                result += $"{"-".PadRight(entry.Width + spacer, '-')}{separator}";
            }
        }

        return result;
    }

    /// <summary>
    /// Prints the header line
    /// </summary>
    /// <param name="widthList">The list with the column width</param>
    /// <returns>The header line</returns>
    public static string PrintHeaderLine(IEnumerable<ColumnWidth> widthList)
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
    public static string PrintLine(bool printLineNumbers, int maxLineLength, IEnumerable<ColumnWidth> widthList, LineEntry line, bool header)
    {
        const string lineStartEnd = "|";

        var result = lineStartEnd;

        if (printLineNumbers)
        {
            if (header)
                result += $" {"Row".PadRight(maxLineLength)} {lineStartEnd}";
            else
                result += $" {line.Id.ToString().PadLeft(maxLineLength)} {lineStartEnd}";
        }

        foreach (var entry in widthList)
        {
            var value = line.Values.FirstOrDefault(f => f.ColumnName.Equals(entry.ColumnName));
            if (value == null)
                continue;

            if (header)
            {
                result += $" {value.DisplayName.PadRight(entry.Width)} {lineStartEnd}";
            }
            else
            {
                result += entry.Align == TextAlign.Left
                    ? $" {value.Value.PadRight(entry.Width)} {lineStartEnd}"
                    : $" {value.Value.PadLeft(entry.Width)} {lineStartEnd}";
            }
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
        return (from property in properties
                let maxValue = printList.SelectMany(s => s.Values)
                    .Where(w => w.ColumnName.Equals(property.Name))
                    .Max(m => m.Value.Length)
                select new ColumnWidth(property.Name, maxValue, property.Appearance?.TextAlign ?? TextAlign.Left))
            .ToList();
    }

    /// <summary>
    /// Gets the value of the property
    /// </summary>
    /// <param name="obj">The object which contains the data</param>
    /// <param name="propertyName">The name of the property</param>
    /// <param name="format">The desired format</param>
    /// <returns>The property value</returns>
    public static string GetPropertyValue(object obj, string propertyName, string format = "")
    {
        var value = obj.GetType().GetProperty(propertyName) == null
            ? null
            : obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);

        if (value == null)
            return string.Empty;

        var tmpValue = value.ToString() ?? string.Empty;
        return string.IsNullOrEmpty(format) ? tmpValue : string.Format($"{{0:{format}}}", tmpValue);
    }

    /// <summary>
    /// Gets the value of the property
    /// </summary>
    /// <param name="row">The data row</param>
    /// <param name="propertyName">The name of the property aka column name</param>
    /// <param name="format">The desired format</param>
    /// <returns>The property value</returns>
    public static string GetPropertyValue(DataRow row, string propertyName, string format = "")
    {
        var value = row[propertyName].ToString();

        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return string.IsNullOrEmpty(format) ? value : string.Format($"{{0:{format}}}", value);
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

        if (overrideList == null || !overrideList.Any())
            return values.Where(w => !w.Ignore).OrderBy(o => o.Order).ToList();

        foreach (var entry in values)
        {
            var overrideEntry = overrideList.FirstOrDefault(f => f.PropertyName.Equals(entry.Name, StringComparison.OrdinalIgnoreCase));
            if (overrideEntry != null)
                entry.Appearance = overrideEntry.Appearance;
        }

        return values.Where(w => !w.Ignore).OrderBy(o => o.Order).ToList();
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

        return values.Where(w => !w.Ignore).OrderBy(o => o.Order).ToList();
    }

    /// <summary>
    /// Checks if the type is a "list"
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <returns><see langword="true"/> when the type is a list, otherwise <see langword="false"/></returns>
    public static bool IsList<T>()
    {
        var type = typeof(T);
        return typeof(IEnumerable).IsAssignableFrom(type);
    }
}