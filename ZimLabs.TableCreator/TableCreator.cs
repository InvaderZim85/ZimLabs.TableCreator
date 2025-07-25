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
    /// Converts the given list into a "table" or a CSV list.
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="list">The list with the values.</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <returns>The created table</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static string CreateTable<T>(this IEnumerable<T> list, TableCreatorOptions options) where T : class
    {
        // Check if the list is "null"
        ArgumentNullException.ThrowIfNull(list);

        if (options.OutputType == OutputType.Csv)
            return Helper.CreateCsv(list, options);

        // Get the properties of the given type
        var properties = Helper.GetProperties<T>(options.OverrideList);

        // Create the temp list
        var printList = new List<LineEntry>();

        // Add the header to the list
        var headerLine = new LineEntry(0, true)
        {
            // Add the columns to the header
            Values = [.. properties.Select(s => new ValueEntry(s))]
        };

        printList.Add(headerLine);

        // Add the values to the list
        var count = 1;
        printList.AddRange(list.Select(entry => new LineEntry(count++)
        {
            Values = [.. properties.Select(s => new ValueEntry(s.Name, Helper.GetPropertyValue(entry, s, false, options.EncapsulateText)))]
        }));

        // 3 = "Row"
        var tmpLength = count.ToString().Length;
        var maxLineLength = tmpLength > 3 ? tmpLength : 3;

        // Get the max values
        var widthList = Helper.GetColumnWidthList(properties, printList);

        return Helper.CreateTable(options.OutputType, options.PrintLineNumbers, maxLineLength, widthList,
            printList);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <typeparam name="T">The type of the values.</typeparam>
    /// <param name="list">The list with the values.</param>
    /// <param name="filepath">The path of the destination file.</param>
    /// <param name="options">The options for the creation of the table.</param>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/>.</exception>
    public static void SaveTable<T>(this IEnumerable<T> list, string filepath, TableCreatorOptions options)
        where T : class
    {
        var result = CreateTable(list, options);

        File.WriteAllText(filepath, result, options.Encoding);
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
    public static async Task SaveTableAsync<T>(this IEnumerable<T> list, string filepath, TableCreatorOptions options)
        where T : class
    {
        var result = CreateTable(list, options);

        await File.WriteAllTextAsync(filepath, result, options.Encoding);
    }

    #endregion

    #region Publich methods for DataTable

    /// <summary>
    /// Converts the given list into a "table"
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <returns>The created table</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static string CreateTable(this DataTable table, TableCreatorOptions options)
    {
        ArgumentNullException.ThrowIfNull(table);

        if (options.OutputType == OutputType.Csv)
            return Helper.CreateCsv(table, options);

        // Get the properties of the given type
        var properties = Helper.GetProperties(table, options.OverrideList);

        // Create the temp list
        var printList = new List<LineEntry>();

        // Add the header to the list
        var headerLine = new LineEntry(0, true)
        {
            // Add the columns to the header
            Values = [.. properties.Select(s => new ValueEntry(s))]
        };

        printList.Add(headerLine);

        // Add the values to the list
        var count = 1;
        printList.AddRange(from DataRow row in table.Rows
            select new LineEntry(count++)
            {
                Values = [.. properties.Select(s => new ValueEntry(s.Name, Helper.GetPropertyValue(row, s.Name, s.Appearance.Format, false)))]
            });

        // 3 chars for "row"
        var maxLineLength = count.ToString().Length > 3 ? count.ToString().Length : 3;

        // Get the max values
        var widthList = Helper.GetColumnWidthList(properties, printList);

        return Helper.CreateTable(options.OutputType, options.PrintLineNumbers, maxLineLength, widthList,
            printList);
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
        var result = CreateTable(table, options);

        File.WriteAllText(filepath, result, options.Encoding);
    }

    /// <summary>
    /// Converts the given list into a "table" and save it into the specified file
    /// </summary>
    /// <param name="table">The table with the data</param>
    /// <param name="filepath">The path of the destination file</param>
    /// <param name="options">The options for the creation of the table</param>
    /// <returns>The awaitable task</returns>
    /// <exception cref="ArgumentNullException">Will be thrown if the provided list is <see langword="null"/></exception>
    public static async Task SaveTableAsync(this DataTable table, string filepath, TableCreatorOptions options)
    {
        var result = CreateTable(table, options);

        await File.WriteAllTextAsync(filepath, result, options.Encoding);
    }

    #endregion

    #region Public methods for objects

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
        Helper.ThrowIfNotSupported<T>();

        ArgumentNullException.ThrowIfNull(value);

        // Get the properties
        var properties = Helper.GetProperties<T>(options.OverrideList);
        var maxLength = properties.Select(s => s.CustomName).Max(m => m.Length);

        var sb = new StringBuilder();

        var count = 1;
        foreach (var property in properties)
        {
            var listIndicator = options.ListType == ListType.Bullets ? "-" : $"{count++}.";
            var dotLength = options.AlignProperties ? maxLength - property.CustomName.Length : 0;
            sb.AppendLine(
                $"{listIndicator} {property.CustomName}{"".PadRight(dotLength, '.')}: " +
                $"{Helper.GetPropertyValue(value, property, false, false)}");
        }

        return sb.ToString();
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
        Helper.ThrowIfNotSupported<T>();

        ArgumentNullException.ThrowIfNull(value);

        var properties = Helper.GetProperties<T>(options.OverrideList);

        var data = (from property in properties
            let tmpValue = Helper.GetPropertyValue(value, property, options.OutputType == OutputType.Csv, options.EncapsulateText)
            select new KeyValueEntry(property.CustomName, tmpValue)).ToList();

        return CreateTable(data, options);
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
        var result = value.CreateValueList(options);

        File.WriteAllText(filepath, result, options.Encoding);
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
        Helper.ThrowIfNotSupported<T>();

        ArgumentNullException.ThrowIfNull(value);

        var properties = Helper.GetProperties<T>(options.OverrideList);

        var data = properties.Select(s => new
        {
            Key = s.CustomName,
            Value = Helper.GetPropertyValue(value, s, options.OutputType == OutputType.Csv, options.EncapsulateText)
        });

        data.SaveTable(filepath, options);
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
    public static async Task SaveValueAsync<T>(this T value, string filepath, TableCreatorListOptions options) where T : class
    {
        var result = value.CreateValueList(options);

        await File.WriteAllTextAsync(filepath, result, options.Encoding);
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
    public static async Task SaveValueAsTableAsync<T>(this T value, string filepath, TableCreatorOptions options)
        where T : class
    {
        Helper.ThrowIfNotSupported<T>();

        ArgumentNullException.ThrowIfNull(value);

        var properties = Helper.GetProperties<T>(options.OverrideList);

        var data = properties.Select(s => new
        {
            Key = s.CustomName,
            Value = Helper.GetPropertyValue(value, s, options.OutputType == OutputType.Csv, options.EncapsulateText)
        });

        await data.SaveTableAsync(filepath, options);
    }

    #endregion

    
}