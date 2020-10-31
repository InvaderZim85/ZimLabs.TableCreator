# TableCreator.CreateTable&lt;T&gt; method

Converts the given list into a "table"

```csharp
public static string CreateTable<T>(IEnumerable<T> list, 
    OutputType outputType = OutputType.Default, bool printLineNumbers = false)
    where T : class
```

| parameter | description |
| --- | --- |
| T | The type of the values |
| list | The list with the values |
| outputType | The desired output type (optional) |
| printLineNumbers | true to print line numbers, otherwise false |

## See Also

* enum [OutputType](../OutputType.md)
* class [TableCreator](../TableCreator.md)
* namespace [ZimLabs.TableCreator](../../ZimLabs.TableCreator.md)

<!-- DO NOT EDIT: generated by xmldocmd for ZimLabs.TableCreator.dll -->