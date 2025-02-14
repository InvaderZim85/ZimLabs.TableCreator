using BenchmarkDotNet.Running;
using ZimLabs.TableCreator;
using ZimLabs.TableCreator.DataObjects;

namespace Demo;

internal static class Program
{
    private static void Main()
    {
        TableTest();

        ListTest();

        SingleClassEntryTest();

        AnonymousTest();

        SingleAnonymousEntryTest();

        EmptyListTest();

        EmptyEntryTest();

        WrongMethodTest();

        CsvEncapsulateContentTest();

        // Run the benchmark
        Console.WriteLine("Benchmarks");
        Console.WriteLine("==========");
        Console.WriteLine();
        BenchmarkRunner.Run<BenchmarkTest>();
    }

    private static void TableTest()
    {
        // Create a table
        var dataTable = Helper.CreatePersonTable();

        // Add some attributes to override the original behaviour
        var overrideList = new List<OverrideAttributeEntry>
        {
            new("Id", new AppearanceAttribute(order: 1, textAlign: TextAlign.Left)),
            new("Name", new AppearanceAttribute(order: 3, encapsulateContent: true)),
            new("Title", new AppearanceAttribute(order: 2, encapsulateContent: true)),
            new("DepartmentId", new AppearanceAttribute(true)),
            new("Locked", new AppearanceAttribute("User locked", order: 4)),
            new("SomeValue", new AppearanceAttribute("Some value", order: 5, format: "N0")),
            new("SomeDate", new AppearanceAttribute("Some date", order: 6, format: "yyyy-MM-dd HH:mm:ss"))
        };

        Console.WriteLine("DataTable Test");
        Console.WriteLine("==============");
        foreach (var outputType in Enum.GetValues<OutputType>())
        {
            var tmpValue = $"Output type: {outputType}";
            Console.WriteLine(tmpValue);
            Console.WriteLine("-".PadRight(tmpValue.Length, '-'));
            Console.WriteLine(dataTable.CreateTable(new TableCreatorOptions
            {
                OutputType = outputType
            }));
        }
    }

    private static void ListTest()
    {
        var dummyList = Helper.CreatePersonList();

        Console.WriteLine("Class list");
        Console.WriteLine("==========");
        foreach (var outputType in Enum.GetValues<OutputType>())
        {
            var tmpValue = $"Output type: {outputType}";
            Console.WriteLine(tmpValue);
            Console.WriteLine("-".PadRight(tmpValue.Length, '-'));
            Console.WriteLine(dummyList.CreateTable(new TableCreatorOptions
            {
                OutputType = outputType
            }));
        }
    }

    private static void SingleClassEntryTest()
    {
        var entry = Helper.CreatePersonEntry();

        Console.WriteLine("Single class entry (with alignment)");
        Console.WriteLine("===================================");
        foreach (var listType in Enum.GetValues<ListType>())
        {
            var tmpValue = $"List type: {listType}";
            Console.WriteLine(tmpValue);
            Console.WriteLine("-".PadRight(tmpValue.Length, '-'));
            Console.WriteLine(entry.CreateValueList(new TableCreatorListOptions
            {
                ListType = listType,
                AlignProperties = true
            }));
        }

        Console.WriteLine("Single class entry (w/o alignment)");
        Console.WriteLine("==================================");
        foreach (var listType in Enum.GetValues<ListType>())
        {
            var tmpValue = $"List type: {listType}";
            Console.WriteLine(tmpValue);
            Console.WriteLine("-".PadRight(tmpValue.Length, '-'));
            Console.WriteLine(entry.CreateValueList(new TableCreatorListOptions
            {
                ListType = listType
            }));
        }
    }

    private static void AnonymousTest()
    {
        var list = Helper.CreatePersonList().Select(s => new
        {
            s.Id,
            s.Name,
            s.Title,
            s.DepartmentId,
            s.Locked
        }).ToList();

        Console.WriteLine("Anonymous list");
        Console.WriteLine("==============");
        foreach (var outputType in Enum.GetValues<OutputType>())
        {
            var tmpValue = $"Output type: {outputType}";
            Console.WriteLine(tmpValue);
            Console.WriteLine("-".PadRight(tmpValue.Length, '-'));
            Console.WriteLine(list.CreateTable(new TableCreatorOptions
            {
                OutputType = outputType
            }));
        }
    }

    private static void SingleAnonymousEntryTest()
    {
        var entry = Helper.CreatePersonList().Select(s => new
        {
            s.Id,
            s.Name,
            s.Title,
            s.DepartmentId,
            s.Locked
        }).FirstOrDefault();

        Console.WriteLine("Single anonymous entry test (with alignment)");
        Console.WriteLine("============================================");
        foreach (var listType in Enum.GetValues<ListType>())
        {
            var tmpValue = $"List type: {listType}";
            Console.WriteLine(tmpValue);
            Console.WriteLine("-".PadRight(tmpValue.Length, '-'));
            Console.WriteLine(entry!.CreateValueList(new TableCreatorListOptions
            {
                ListType = listType,
                AlignProperties = true
            }));
        }

        Console.WriteLine("Single anonymous entry test (w/o alignment)");
        Console.WriteLine("===========================================");
        foreach (var listType in Enum.GetValues<ListType>())
        {
            var tmpValue = $"List type: {listType}";
            Console.WriteLine(tmpValue);
            Console.WriteLine("-".PadRight(tmpValue.Length, '-'));
            Console.WriteLine(entry!.CreateValueList(new TableCreatorListOptions
            {
                ListType = listType
            }));
        }
    }

    private static void EmptyListTest()
    {
        Console.WriteLine("Empty list test");
        Console.WriteLine("===============");
        List<Person>? emptyList = null;

        try
        {
            _ = emptyList!.CreateTable(new TableCreatorOptions());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Empty list test failed. Message: {ex.Message}");
        }

        Console.WriteLine();
    }

    private static void EmptyEntryTest()
    {
        Console.WriteLine("Empty entry test");
        Console.WriteLine("================");

        Person? person = null;

        try
        {
            _ = person!.CreateValueTable(new TableCreatorOptions());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Empty entry test failed. Message: {ex.Message}");
        }

        Console.WriteLine();
    }

    private static void WrongMethodTest()
    {
        Console.WriteLine("Wrong method test");
        Console.WriteLine("=================");

        var persons = Helper.CreatePersonList();

        try
        {
            persons.CreateValueList(new TableCreatorListOptions());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wrong method test failed: Message: {ex.Message}");
        }

        Console.WriteLine();
    }

    private static void CsvEncapsulateContentTest()
    {
        Console.WriteLine("CSV - Encapsulate content test (only name)");
        Console.WriteLine("==========================================");

        var persons = Helper.CreatePersonList();

        var content = persons.CreateTable(new TableCreatorOptions
        {
            OutputType = OutputType.Csv
        });

        Console.WriteLine(content);

        Console.WriteLine("CSV - Encapsulate content test (all text values)");
        Console.WriteLine("================================================");

        content = persons.CreateTable(new TableCreatorOptions
        {
            OutputType = OutputType.Csv,
            EncapsulateText = true
        });

        Console.WriteLine(content);
    }
}

