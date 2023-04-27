using System.Collections.Generic;
using System.Data;
using System.Text;
using ZimLabs.TableCreator.DataObjects;

namespace ZimLabs.TableCreator;

/// <summary>
/// Provides the function to convert a list of "objects" into a "table" (ASCII style, markdown, csv)
/// </summary>
public static class TableCreator
{
    #region Public methods for lists

    /// <summary>
    /// Converts the given list into a "table"
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="list">The list with the values</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The created table</returns>
    public static string CreateTable<T>(this IEnumerable<T> list, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        if (outputType == OutputType.Csv)
            return TableHelper.CreateCsv(list, delimiter, printLineNumbers, overrideList);

        // Get the properties of the given type
        var properties = TableHelper.GetProperties<T>(overrideList);

        // Create the temp list
        var printList = new List<LineEntry>();

        // Add the header to the list
        var headerLine = new LineEntry(0, true);

        // Add the columns to the header
        foreach (var property in properties)
        {
            if (string.IsNullOrEmpty(property.Appearance.Name))
                headerLine.Values.Add(new ValueEntry(property.Name, property.Name));
            else
            {
                headerLine.Values.Add(new ValueEntry(property.Name, property.Appearance.Name,
                    property.Appearance.Name));
            }
        }

        printList.Add(headerLine);

        // Add the values to the list
        var count = 1;
        foreach (var entry in list)
        {
            var lineEntry = new LineEntry(count++);

            foreach (var property in properties)
            {
                lineEntry.Values.Add(new ValueEntry(property.Name,
                    TableHelper.GetPropertyValue(entry, property.Name, property.Appearance.Format)));
            }

            printList.Add(lineEntry);
        }

        // 3 = "Row"
        var maxLineLength = count.ToString().Length > 3 ? count.ToString().Length : 3;

        // Get the max values
        var widthList = TableHelper.GetColumnWidthList(properties, printList);

        return TableHelper.CreateTable(outputType, printLineNumbers, maxLineLength, widthList, printList);
    }

    /// <summary>
    /// Converts the given list into a "table"
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The created table</returns>
    public static string CreateTable(this DataTable table, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
    {
        if (outputType == OutputType.Csv)
            return TableHelper.CreateCsv(table, delimiter, printLineNumbers, overrideList);

        // Get the properties of the given type
        var properties = TableHelper.GetProperties(table, overrideList);

        // Create the temp list
        var printList = new List<LineEntry>();

        // Add the header to the list
        var headerLine = new LineEntry(0, true);

        // Add the columns to the header
        foreach (var property in properties)
        {
            if (string.IsNullOrEmpty(property.Appearance.Name))
                headerLine.Values.Add(new ValueEntry(property.Name, property.Name));
            else
            {
                headerLine.Values.Add(new ValueEntry(property.Name, property.Appearance.Name,
                    property.Appearance.Name));
            }
        }

        printList.Add(headerLine);

        // Add the values to the list
        var count = 1;
        foreach (DataRow row in table.Rows)
        {
            var lineEntry = new LineEntry(count++);

            foreach (var property in properties)
            {
                lineEntry.Values.Add(new ValueEntry(property.Name,
                    TableHelper.GetPropertyValue(row, property.Name, property.Appearance.Format)));
            }

            printList.Add(lineEntry);
        }

        // 3 chars for "row"
        var maxLineLength = count.ToString().Length > 3 ? count.ToString().Length : 3;

        // Get the max values
        var widthList = TableHelper.GetColumnWidthList(properties, printList);

        return TableHelper.CreateTable(outputType, printLineNumbers, maxLineLength, widthList, printList);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="list">The list with the values</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    public static void SaveTable<T>(this IEnumerable<T> list, string filepath,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        list.SaveTable(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, overrideList);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="list">The list with the values</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    public static async Task SaveTableAsync<T>(this IEnumerable<T> list, string filepath,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        await list.SaveTableAsync(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, overrideList);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    public static void SaveTable(this DataTable table, string filepath,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
    {
        table.SaveTable(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, overrideList);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    public static async Task SaveTableAsync(this DataTable table, string filepath,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
    {
        await table.SaveTableAsync(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, overrideList);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="list">The list with the values</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    public static void SaveTable<T>(this IEnumerable<T> list, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = CreateTable(list, outputType, printLineNumbers, delimiter, overrideList);

        File.WriteAllText(filepath, result, encoding);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    public static void SaveTable(this DataTable table, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
    {
        var result = CreateTable(table, outputType, printLineNumbers, delimiter, overrideList);

        File.WriteAllText(filepath, result, encoding);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="list">The list with the values</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    public static async Task SaveTableAsync<T>(this IEnumerable<T> list, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = CreateTable(list, outputType, printLineNumbers, delimiter, overrideList);

        await File.WriteAllTextAsync(filepath, result, encoding);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    public static async Task SaveTableAsync(this DataTable table, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
    {
        var result = CreateTable(table, outputType, printLineNumbers, delimiter, overrideList);

        await File.WriteAllTextAsync(filepath, result, encoding);
    }

    #endregion

    #region Public methods for objects

    /// <summary>
    /// Creates a list of the properties with its values
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties">true to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The list</returns>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    public static string CreateValueList<T>(this T value, ListType type = ListType.Bullets,
        bool alignProperties = false, List<OverrideAttributeEntry>? overrideList = null) 
        where T : class
    {
        if (TableHelper.IsList<T>())
        {
            throw new NotSupportedException(
                "The specified type is not supported by this method. Please choose \"CreateTable\" or \"SaveTable\" instead.");
        }

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        // Get the properties
        var properties = TableHelper.GetProperties<T>(overrideList);
        var maxLength = properties.Select(s => s.CustomName).Max(m => m.Length);

        var sb = new StringBuilder();

        var count = 1;
        foreach (var property in properties)
        {
            var listIndicator = type == ListType.Bullets ? "-" : $"{count++}.";
            var dotLength = alignProperties ? maxLength - property.CustomName.Length : 0;
            sb.AppendLine(
                $"{listIndicator} {property.CustomName}{"".PadRight(dotLength, '.')}: " +
                $"{TableHelper.GetPropertyValue(value, property.Name, property.Appearance.Format)}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Converts the given value into a "table" (Key, Value columns)
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="value">The value</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The created table</returns>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    public static string CreateValueTable<T>(this T value, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        if (TableHelper.IsList<T>())
        {
            throw new NotSupportedException(
                "The specified type is not supported by this method. Please choose \"CreateTable\" or \"SaveTable\" instead.");
        }

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var properties = TableHelper.GetProperties<T>(overrideList);

        var data = (from property in properties
                    let tmpValue = TableHelper.GetPropertyValue(value, property.Name, property.Appearance.Format)
                    select new KeyValueEntry
                    {
                        Key = property.CustomName,
                        Value = tmpValue
                    }).ToList();

        return CreateTable(data, outputType, printLineNumbers, delimiter);
    }

    /// <summary>
    /// Creates a list of the properties with its values and saves it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties">true to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    public static void SaveValue<T>(this T value, string filepath, ListType type = ListType.Bullets,
        bool alignProperties = false, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = value.CreateValueList(type, alignProperties, overrideList);

        File.WriteAllText(filepath, result, Encoding.UTF8);
    }

    /// <summary>
    /// Creates a list of the properties with its values and saves it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties">true to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    /// <returns>The awaitable task</returns>
    public static async Task SaveValueAsync<T>(this T value, string filepath, ListType type = ListType.Bullets,
        bool alignProperties = false, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = value.CreateValueList(type, alignProperties, overrideList);

        await File.WriteAllTextAsync(filepath, result, Encoding.UTF8);
    }

    /// <summary>
    /// Creates a list of the properties with its values and saves it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties">true to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    public static void SaveValue<T>(this T value, string filepath, Encoding encoding, ListType type = ListType.Bullets,
        bool alignProperties = false, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = value.CreateValueList(type, alignProperties, overrideList);

        File.WriteAllText(filepath, result, encoding);
    }

    /// <summary>
    /// Creates a list of the properties with its values and saves it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties">true to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    /// <returns>The awaitable task</returns>
    public static async Task SaveValueAsync<T>(this T value, string filepath, Encoding encoding, ListType type = ListType.Bullets,
        bool alignProperties = false, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = value.CreateValueList(type, alignProperties, overrideList);

        await File.WriteAllTextAsync(filepath, result, encoding);
    }

    /// <summary>
    /// Converts the given value into a "table" (Key, Value column) and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    public static void SaveValueAsTable<T>(this T value, string filepath, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        value.SaveValueAsTable(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, overrideList);
    }

    /// <summary>
    /// Converts the given value into a "table" (Key, Value column) and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    /// <returns>The awaitable task</returns>
    public static async Task SaveValueAsTableAsync<T>(this T value, string filepath, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        if (value == null)
            throw new ArgumentNullException(nameof(value));

        await value.SaveValueAsTableAsync(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, overrideList);
    }

    /// <summary>
    /// Converts the given value into a "table" (Key, Value column) and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    public static void SaveValueAsTable<T>(this T value, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
    {
        if (TableHelper.IsList<T>())
        {
            throw new NotSupportedException(
                "The specified type is not supported by this method. Please choose \"CreateTable\" or \"SaveTable\" instead.");
        }

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var properties = TableHelper.GetProperties<T>(overrideList);

        var data = properties.Select(s => new
        {
            Key = s.CustomName,
            Value = TableHelper.GetPropertyValue(value, s.Name)
        });

        data.SaveTable(filepath, encoding, outputType, printLineNumbers, delimiter);
    }

    /// <summary>
    /// Converts the given value into a "table" (Key, Value column) and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is a assignable from IEnumerable</exception>
    /// <returns>The awaitable task</returns>
    public static async Task SaveValueAsTableAsync<T>(this T value, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", List<OverrideAttributeEntry>? overrideList = null)
    {
        if (TableHelper.IsList<T>())
        {
            throw new NotSupportedException(
                "The specified type is not supported by this method. Please choose \"CreateTable\" or \"SaveTable\" instead.");
        }

        if (value == null)
            throw new ArgumentNullException(nameof(value));

        var properties = TableHelper.GetProperties<T>(overrideList);

        var data = properties.Select(s => new
        {
            Key = s.CustomName,
            Value = TableHelper.GetPropertyValue(value, s.Name)
        });

        await data.SaveTableAsync(filepath, encoding, outputType, printLineNumbers, delimiter);
    }

    #endregion
}