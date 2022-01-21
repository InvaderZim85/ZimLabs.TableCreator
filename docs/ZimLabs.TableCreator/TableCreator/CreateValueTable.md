# TableCreator.CreateValueTable&lt;T&gt; method

Converts the given value into a "table" (Key, Value columns)

```csharp
public static string CreateValueTable<T>(this T value, OutputType outputType = OutputType.Default, 
    bool printLineNumbers = false, string delimiter = ";")
    where T : class
```

| parameter | description |
| --- | --- |
| T | The type of the values |
| value | The value |
| outputType | The desired output type (optional) |
| printLineNumbers | true to print line numbers, otherwise false (optional) |
| delimiter | The delimiter which should be used for CSV (only needed when *outputType* is set to Csv) |

## Return Value

The created table

## Exceptions

| exception | condition |
| --- | --- |
| ArgumentNullException | Will be thrown when the value is null |

## See Also

* enum [OutputType](../OutputType.md)
* class [TableCreator](../TableCreator.md)
* namespace [ZimLabs.TableCreator](../../ZimLabs.TableCreator.md)

<!-- DO NOT EDIT: generated by xmldocmd for ZimLabs.TableCreator.dll -->