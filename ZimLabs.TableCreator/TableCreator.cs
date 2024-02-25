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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The created table</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static string CreateTable<T>(this IEnumerable<T> list, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        // Check if the list is "null"
        ArgumentNullException.ThrowIfNull(list);

        if (outputType == OutputType.Csv)
            return TableHelper.CreateCsv(list, delimiter, printLineNumbers, addHeader, overrideList);

        // Get the properties of the given type
        var properties = TableHelper.GetProperties<T>(overrideList);

        // Create the temp list
        var printList = new List<LineEntry>();

        // Add the header to the list
        var headerLine = new LineEntry(0, true)
        {
            // Add the columns to the header
            Values = properties.Select(s => new ValueEntry(s)).ToList()
        };
        
        printList.Add(headerLine);

        // Add the values to the list
        var count = 1;
        printList.AddRange(list.Select(entry => new LineEntry(count++)
        {
            Values = properties.Select(s => new ValueEntry(s.Name, TableHelper.GetPropertyValue(entry, s, false))).ToList()
        }));

        // 3 = "Row"
        var tmpLength = count.ToString().Length;
        var maxLineLength = tmpLength > 3 ? tmpLength : 3;

        // Get the max values
        var widthList = TableHelper.GetColumnWidthList(properties, printList);

        return TableHelper.CreateTable(outputType, printLineNumbers, maxLineLength, widthList, printList);
    }

    /// <summary>
    /// Converts the given list into a "table"
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="list">The list with the values</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <returns>The created table</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static string CreateTable<T>(this IEnumerable<T> list, TableCreatorOptions options) where T : class
    {
        return CreateTable(list, options.OutputType, options.PrintLineNumbers, options.Delimiter, options.AddHeader, options.OverrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static void SaveTable<T>(this IEnumerable<T> list, string filepath,
        OutputType outputType = OutputType.Default, bool printLineNumbers = false, string delimiter = ";",
        bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        list.SaveTable(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, addHeader, overrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static void SaveTable<T>(this IEnumerable<T> list, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default, bool printLineNumbers = false, string delimiter = ";",
        bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = CreateTable(list, outputType, printLineNumbers, delimiter, addHeader, overrideList);

        File.WriteAllText(filepath, result, encoding);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="list">The list with the values</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static void SaveTable<T>(this IEnumerable<T> list, string filepath, TableCreatorOptions options)
        where T : class
    {
        SaveTable(list, filepath, options.Encoding, options.OutputType, options.PrintLineNumbers, options.Delimiter, options.AddHeader, options.OverrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static Task SaveTableAsync<T>(this IEnumerable<T> list, string filepath,
        OutputType outputType = OutputType.Default, bool printLineNumbers = false, string delimiter = ";",
        bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        return list.SaveTableAsync(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, addHeader,
            overrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static Task SaveTableAsync<T>(this IEnumerable<T> list, string filepath, Encoding encoding, OutputType outputType = OutputType.Default, bool printLineNumbers = false, string delimiter = ";", bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = CreateTable(list, outputType, printLineNumbers, delimiter, addHeader, overrideList);

        return File.WriteAllTextAsync(filepath, result, encoding);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="list">The list with the values</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static Task SaveTableAsync<T>(this IEnumerable<T> list, string filepath, TableCreatorOptions options)
        where T : class
    {
        return SaveTableAsync(list, filepath, options.Encoding, options.OutputType, options.PrintLineNumbers,
            options.Delimiter, options.AddHeader, options.OverrideList);
    }

    #endregion

    #region Publich methods for DataTable

    /// <summary>
    /// Converts the given list into a "table"
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The created table</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static string CreateTable(this DataTable table, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
    {
        ArgumentNullException.ThrowIfNull(table);

        if (outputType == OutputType.Csv)
            return TableHelper.CreateCsv(table, delimiter, printLineNumbers, addHeader, overrideList);

        // Get the properties of the given type
        var properties = TableHelper.GetProperties(table, overrideList);

        // Create the temp list
        var printList = new List<LineEntry>();

        // Add the header to the list
        var headerLine = new LineEntry(0, true)
        {
            // Add the columns to the header
            Values = properties.Select(s => new ValueEntry(s)).ToList()
        };

        printList.Add(headerLine);

        // Add the values to the list
        var count = 1;
        printList.AddRange(from DataRow row in table.Rows
            select new LineEntry(count++)
            {
                Values = properties.Select(s =>
                    new ValueEntry(s.Name, TableHelper.GetPropertyValue(row, s.Name, s.Appearance.Format, false))).ToList()
            });

        // 3 chars for "row"
        var maxLineLength = count.ToString().Length > 3 ? count.ToString().Length : 3;

        // Get the max values
        var widthList = TableHelper.GetColumnWidthList(properties, printList);

        return TableHelper.CreateTable(outputType, printLineNumbers, maxLineLength, widthList, printList);
    }

    /// <summary>
    /// Converts the given list into a "table"
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <returns>The created table</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static string CreateTable(this DataTable table, TableCreatorOptions options)
    {
        return CreateTable(table, options.OutputType, options.PrintLineNumbers, options.Delimiter, options.AddHeader,
            options.OverrideList);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static void SaveTable(this DataTable table, string filepath, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", bool addHeader = true,
        List<OverrideAttributeEntry>? overrideList = null)
    {
        table.SaveTable(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, addHeader, overrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static void SaveTable(this DataTable table, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default, bool printLineNumbers = false, string delimiter = ";",
        bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
    {
        var result = CreateTable(table, outputType, printLineNumbers, delimiter, addHeader, overrideList);

        File.WriteAllText(filepath, result, encoding);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static void SaveTable(this DataTable table, string filepath, TableCreatorOptions options)
    {
        SaveTable(table, filepath, options.Encoding, options.OutputType, options.PrintLineNumbers, options.Delimiter,
            options.AddHeader, options.OverrideList);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static Task SaveTableAsync(this DataTable table, string filepath, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", bool addHeader = true,
        List<OverrideAttributeEntry>? overrideList = null)
    {
        return table.SaveTableAsync(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, addHeader,
            overrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static Task SaveTableAsync(this DataTable table, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default, bool printLineNumbers = false, string delimiter = ";",
        bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
    {
        var result = CreateTable(table, outputType, printLineNumbers, delimiter, addHeader, overrideList);

        return File.WriteAllTextAsync(filepath, result, encoding);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static Task SaveTableAsync(this DataTable table, string filepath, TableCreatorOptions options)
    {
        return SaveTableAsync(table, filepath, options.Encoding, options.OutputType, options.PrintLineNumbers,
            options.Delimiter, options.AddHeader, options.OverrideList);
    }

    #endregion

    #region Public methods for objects

    /// <summary>
    /// Creates a list of the properties with its values
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties"><see langword="true"/> to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The list</returns>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static string CreateValueList<T>(this T value, ListType type = ListType.Bullets,
        bool alignProperties = false, List<OverrideAttributeEntry>? overrideList = null) 
        where T : class
    {
        if (TableHelper.IsList<T>())
        {
            throw new NotSupportedException(
                "The specified type is not supported by this method. Please choose \"CreateTable\" or \"SaveTable\" instead.");
        }

        ArgumentNullException.ThrowIfNull(value);

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
                $"{TableHelper.GetPropertyValue(value, property, false)}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Creates a list of the properties with its values
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="options">The options for the list creation</param>
    /// <returns>The list</returns>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static string CreateValueList<T>(this T value, TableCreatorListOptions options) where T : class
    {
        return CreateValueList(value, options.ListType, options.AlignProperties, options.OverrideList);
    }

    /// <summary>
    /// Converts the given value into a "table" (Key, Value columns)
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="value">The value</param>
    /// <param name="outputType">The desired output type (optional)</param>
    /// <param name="printLineNumbers">true to print line numbers, otherwise false (optional)</param>
    /// <param name="delimiter">The delimiter which should be used for CSV (only needed when <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/>)</param>
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The created table</returns>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static string CreateValueTable<T>(this T value, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", bool addHeader = true,
        List<OverrideAttributeEntry>? overrideList = null) where T : class
    {
        if (TableHelper.IsList<T>())
        {
            throw new NotSupportedException(
                "The specified type is not supported by this method. Please choose \"CreateTable\" or \"SaveTable\" instead.");
        }

        ArgumentNullException.ThrowIfNull(value);

        var properties = TableHelper.GetProperties<T>(overrideList);

        var data = (from property in properties
            let tmpValue = TableHelper.GetPropertyValue(value, property, outputType == OutputType.Csv)
            select new KeyValueEntry(property.CustomName, tmpValue)).ToList();

        return CreateTable(data, outputType, printLineNumbers, delimiter, addHeader, overrideList);
    }

    /// <summary>
    /// Converts the given value into a "table" (Key, Value columns)
    /// </summary>
    /// <typeparam name="T">The type of the values</typeparam>
    /// <param name="value">The value</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <returns>The created table</returns>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static string CreateValueTable<T>(this T value, TableCreatorOptions options) where T : class
    {
        return CreateValueTable(value, options.OutputType, options.PrintLineNumbers, options.Delimiter, options.AddHeader, options.OverrideList);
    }

    /// <summary>
    /// Creates a list of the properties with its values and saves it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties"><see langword="true"/> to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
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
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties"><see langword="true"/> to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
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
    /// <param name="options">The options for the list creation</param>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    public static void SaveValue<T>(this T value, string filepath, TableCreatorListOptions options) where T : class
    {
        SaveValue(value, filepath, options.Encoding, options.ListType, options.AlignProperties, options.OverrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static void SaveValueAsTable<T>(this T value, string filepath, OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(value);

        value.SaveValueAsTable(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, addHeader, overrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static void SaveValueAsTable<T>(this T value, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default,
        bool printLineNumbers = false, string delimiter = ";", bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
    {
        if (TableHelper.IsList<T>())
        {
            throw new NotSupportedException(
                "The specified type is not supported by this method. Please choose \"CreateTable\" or \"SaveTable\" instead.");
        }

        ArgumentNullException.ThrowIfNull(value);

        var properties = TableHelper.GetProperties<T>(overrideList);

        var data = properties.Select(s => new
        {
            Key = s.CustomName,
            Value = TableHelper.GetPropertyValue(value, s, outputType == OutputType.Csv)
        });

        data.SaveTable(filepath, encoding, outputType, printLineNumbers, delimiter, addHeader);
    }

    /// <summary>
    /// Converts the given value into a "table" (Key, Value column) and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static void SaveValueAsTable<T>(this T value, string filepath, TableCreatorOptions options) where T : class
    {
        SaveValueAsTable(value, filepath, options.Encoding, options.OutputType, options.PrintLineNumbers,
            options.Delimiter, options.AddHeader, options.OverrideList);
    }

    /// <summary>
    /// Creates a list of the properties with its values and saves it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties"><see langword="true"/> to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    public static Task SaveValueAsync<T>(this T value, string filepath, ListType type = ListType.Bullets,
        bool alignProperties = false, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = value.CreateValueList(type, alignProperties, overrideList);

        return File.WriteAllTextAsync(filepath, result, Encoding.UTF8);
    }

    /// <summary>
    /// Creates a list of the properties with its values and saves it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="encoding">The encoding of the file</param>
    /// <param name="type">The desired list type</param>
    /// <param name="alignProperties"><see langword="true"/> to add dots to the end of the properties so that all properties have the same length</param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    public static Task SaveValueAsync<T>(this T value, string filepath, Encoding encoding, ListType type = ListType.Bullets,
        bool alignProperties = false, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        var result = value.CreateValueList(type, alignProperties, overrideList);

        return File.WriteAllTextAsync(filepath, result, encoding);
    }

    /// <summary>
    /// Creates a list of the properties with its values and saves it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="options">The options for the list creation</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    /// <exception cref="ArgumentNullException">Will be thrown when the value is null</exception>
    public static Task SaveValueAsync<T>(this T value, string filepath, TableCreatorListOptions options) where T : class
    {
        return SaveValueAsync(value, filepath, options.Encoding, options.ListType, options.AlignProperties,
            options.OverrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static Task SaveValueAsTableAsync<T>(this T value, string filepath,
        OutputType outputType = OutputType.Default, bool printLineNumbers = false, string delimiter = ";",
        bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(value);

        return value.SaveValueAsTableAsync(filepath, Encoding.UTF8, outputType, printLineNumbers, delimiter, addHeader,
            overrideList);
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
    /// <param name="addHeader"><see langword="true"/> to add a header, otherwise <see langword="false"/>. This value is only used when the <paramref name="outputType"/> is set to <see cref="OutputType.Csv"/></param>
    /// <param name="overrideList">The list with the override entries (optional). If you add an entry, the original <see cref="AppearanceAttribute"/> will be ignored</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static Task SaveValueAsTableAsync<T>(this T value, string filepath, Encoding encoding,
        OutputType outputType = OutputType.Default, bool printLineNumbers = false, string delimiter = ";",
        bool addHeader = true, List<OverrideAttributeEntry>? overrideList = null)
    {
        if (TableHelper.IsList<T>())
        {
            throw new NotSupportedException(
                "The specified type is not supported by this method. Please choose \"CreateTable\" or \"SaveTable\" instead.");
        }

        ArgumentNullException.ThrowIfNull(value);

        var properties = TableHelper.GetProperties<T>(overrideList);

        var data = properties.Select(s => new
        {
            Key = s.CustomName,
            Value = TableHelper.GetPropertyValue(value, s, outputType == OutputType.Csv)
        });

        return data.SaveTableAsync(filepath, encoding, outputType, printLineNumbers, delimiter, addHeader);
    }

    /// <summary>
    /// Converts the given value into a "table" (Key, Value column) and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the value</typeparam>
    /// <param name="value">The value</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown when the specified value is null</exception>
    /// <exception cref="NotSupportedException">Will be thrown when the specified type is assignable from IEnumerable</exception>
    public static Task SaveValueAsTableAsync<T>(this T value, string filepath, TableCreatorOptions options)
        where T : class
    {
        return SaveValueAsTableAsync(value, filepath, options.Encoding, options.OutputType, options.PrintLineNumbers,
            options.Delimiter, options.AddHeader, options.OverrideList);
    }

    #endregion
}