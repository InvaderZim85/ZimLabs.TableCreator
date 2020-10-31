using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using ZimLabs.TableCreator.DataObjects;

namespace ZimLabs.TableCreator
{
    /// <summary>
    /// Provides the function to convert a list of objects into a "table" (ASCII style, markdown, csv)
    /// </summary>
    public static class TableCreator
    {
        /// <summary>
        /// Contains the value which indicates if the line numbers should be printed
        /// </summary>
        private static bool _printLineNumbers;

        /// <summary>
        /// Contains the maximal length for the line number
        /// </summary>
        private static int _maxLineLength = 3;

        /// <summary>
        /// Contains the desired table type
        /// </summary>
        private static OutputType _outputType;

        /// <summary>
        /// Converts the given list into a "table"
        /// </summary>
        /// <typeparam name="T">The type of the values</typeparam>
        /// <param name="list">The list with the values</param>
        /// <param name="outputType">The desired output type (optional)</param>
        /// <param name="printLineNumbers">true to print line numbers, otherwise false</param>
        /// <returns></returns>
        public static string CreateTable<T>(IEnumerable<T> list, OutputType outputType = OutputType.Default,
            bool printLineNumbers = false) where T : class
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            _printLineNumbers = printLineNumbers;
            _outputType = outputType;

            // Get the properties of the given type
            var properties = typeof(T).GetProperties();

            // Create the temp list
            var printList = new List<LineEntry>();

            // Add the header to the list
            var headerLine = new LineEntry(0, true);

            // Add the columns to the header
            foreach (var property in properties)
            {
                var attribute = GetAttribute(property);
                if (attribute == null || string.IsNullOrEmpty(attribute.Name))
                    headerLine.Values.Add(new ValueEntry(property.Name, property.Name));
                else
                    headerLine.Values.Add(new ValueEntry(property.Name, attribute.Name, attribute.Name));
            }

            printList.Add(headerLine);

            // Add the values to the list
            var count = 1;
            foreach (var entry in list)
            {
                var lineEntry = new LineEntry(count++);

                foreach (var property in properties)
                {
                    var attribute = GetAttribute(property);

                    if (attribute == null || string.IsNullOrEmpty(attribute.Format))
                        lineEntry.Values.Add(new ValueEntry(property.Name, GetPropertyValue(entry, property.Name)));
                    else
                        lineEntry.Values.Add(new ValueEntry(property.Name,
                            GetPropertyValue(entry, property.Name, attribute.Format)));
                }

                lineEntry.Values.AddRange(
                    properties.Select(s => new ValueEntry(s.Name, GetPropertyValue(entry, s.Name))));

                printList.Add(lineEntry);
            }

            // 3 = "Row"
            _maxLineLength = count.ToString().Length > 3 ? count.ToString().Length : 3;

            // Get the max values
            var widthList = GetColumnWidthList(properties, printList);

            // Create the result
            var result = new StringBuilder();

            // Print the first line (if the default type is chosen)
            if (_outputType == OutputType.Default)
                result.AppendLine(PrintLine(widthList, true));

            // Print the column header
            var header = printList.FirstOrDefault(f => f.IsHeader);
            result.AppendLine(header != null ? PrintLine(widthList, header, true) : PrintHeaderLine(widthList));

            // Print the separator line (if the default type is chosen)
            result.AppendLine(PrintLine(widthList, false));

            // Print the values
            foreach (var line in printList.Where(w => !w.IsHeader).OrderBy(o => o.Id))
            {
                result.AppendLine(PrintLine(widthList, line, false));
            }

            // Print the footer (if the default type is chosen)
            if (_outputType == OutputType.Default)
                result.AppendLine(PrintLine(widthList, true));

            return result.ToString();
        }

        /// <summary>
        /// Gets the appearance attribute of the property
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>The attribute</returns>
        private static AppearanceAttribute GetAttribute(MemberInfo property)
        {
            return property.GetCustomAttribute<AppearanceAttribute>();
        }

        /// <summary>
        /// Prints a single line
        /// </summary>
        /// <param name="widthList">The list with the column width</param>
        /// <param name="firstLastLine">true when the first / last line should be printed</param>
        /// <param name="spacer">The default spacer</param>
        /// <returns>The line</returns>
        private static string PrintLine(IReadOnlyList<ColumnWidth> widthList, bool firstLastLine, int spacer = 2)
        {
            var lineStartEnd = firstLastLine ? "+" : "|";
            if (_outputType == OutputType.Markdown)
                lineStartEnd = "|";

            var result = lineStartEnd;

            if (_printLineNumbers)
            {
                if (_outputType == OutputType.Markdown)
                    result += $"{"-:".PadLeft(_maxLineLength + spacer, '-')}|";
                else
                    result += $"{"-".PadRight(_maxLineLength + spacer, '-')}|";
            }


            for (var i = 0; i < widthList.Count; i++)
            {
                var entry = widthList[i];
                var separator = i + 1 == widthList.Count ? lineStartEnd : "+";
                if (_outputType == OutputType.Markdown)
                {
                    switch (entry.Align)
                    {
                        case TextAlign.Left:
                            result += $"{":-".PadRight(entry.Width + spacer, '-')}|";
                            break;
                        case TextAlign.Right:
                            result += $"{"-:".PadLeft(entry.Width + spacer, '-')}|";
                            break;
                        case TextAlign.Center:
                            result += $":{"-".PadRight(entry.Width, '-')}:|";
                            break;
                        default:
                            result += $"{"-".PadRight(entry.Width + spacer, '-')}|";
                            break;
                    }
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
        private static string PrintHeaderLine(IEnumerable<ColumnWidth> widthList)
        {
            const string lineStartEnd = "|";

            return widthList.Aggregate(lineStartEnd,
                (current, entry) => current + $" {entry.ColumnName.PadRight(entry.Width)} {lineStartEnd}");
        }

        /// <summary>
        /// Prints a single line
        /// </summary>
        /// <param name="widthList">The list with the column width</param>
        /// <param name="line">The line which should be printed</param>
        /// <param name="header">true when the header should be printed</param>
        /// <returns>The value line</returns>
        private static string PrintLine(IEnumerable<ColumnWidth> widthList, LineEntry line, bool header)
        {
            const string lineStartEnd = "|";

            var result = lineStartEnd;

            if (_printLineNumbers)
            {
                if (header)
                    result += $" {"Row".PadRight(_maxLineLength)} {lineStartEnd}";
                else
                    result += $" {line.Id.ToString().PadLeft(_maxLineLength)} {lineStartEnd}";
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
        /// Gets the max length for every column
        /// </summary>
        /// <param name="properties">The list with the properties</param>
        /// <param name="printList">The list with the print entries</param>
        /// <returns>The list with the max length</returns>
        private static List<ColumnWidth> GetColumnWidthList(IEnumerable<PropertyInfo> properties, IReadOnlyCollection<LineEntry> printList)
        {
            return (from property in properties
                    let maxValue = printList.SelectMany(s => s.Values)
                        .Where(w => w.ColumnName.Equals(property.Name))
                        .Max(m => m.Value.Length)
                    let attribute = GetAttribute(property)
                    select new ColumnWidth(property.Name, maxValue, attribute?.TextAlign ?? TextAlign.Left)).ToList();
        }

        /// <summary>
        /// Gets the value of the property
        /// </summary>
        /// <param name="obj">The object which contains the data</param>
        /// <param name="propertyName">The name of the property</param>
        /// <param name="format">The desired format</param>
        /// <returns>The property value</returns>
        private static string GetPropertyValue(object obj, string propertyName, string format = "")
        {
            if (obj == null)
                return "";

            if (format == null)
                format = "";

            var value = obj.GetType().GetProperty(propertyName) == null
                ? null
                : obj.GetType().GetProperty(propertyName)?.GetValue(obj, null);

            if (value == null)
                return null;

            return string.IsNullOrEmpty(format) ? value.ToString() : string.Format($"{{0:{format}}}", value);
        }
    }
}
