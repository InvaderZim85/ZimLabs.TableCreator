using System.Data;
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
    }

    private static void TableTest()
    {
        // Create a table
        var dataTable = new DataTable("Persons");
        dataTable.Columns.Add("Id", typeof(int));
        dataTable.Columns.Add("Name",  typeof(string));
        dataTable.Columns.Add("Title", typeof(string));
        dataTable.Columns.Add("DepartmentId", typeof(int));
        dataTable.Columns.Add("Locked", typeof(bool));

        // Add some attributes to override the original behaviour
        var overrideList = new List<OverrideAttributeEntry>
        {
            new("Id", new AppearanceAttribute
            {
                Order = 1,
                TextAlign = TextAlign.Right
            }),
            new("Name", new AppearanceAttribute
            {
                Order = 3
            }),
            new("Title", new AppearanceAttribute
            {
                Order = 2
            }),
            new("DepartmentId", new AppearanceAttribute
            {
                Ignore = true
            }),
            new("Locked", new AppearanceAttribute
            {
                Name = "User locked",
                Order = 4
            })
        };

        // Add some rows...
        var personList = CreatePersonList();

        foreach (var person in personList)
        {
            dataTable.Rows.Add(person.Id,
                person.Name,
                person.Title,
                person.DepartmentId,
                person.Locked);
        }

        Console.WriteLine("DataTable Test");
        Console.WriteLine("--------------");
        Console.WriteLine("Persons:");
        Console.WriteLine(dataTable.CreateTable(overrideList: overrideList));

        // csv content
        var csvContent = dataTable.CreateTable(OutputType.Csv, overrideList: overrideList);
        Console.WriteLine("CSV content:");
        Console.WriteLine(csvContent);
    }

    private static void ListTest()
    {
        var dummyList = CreatePersonList();

        Console.WriteLine("Class list");
        Console.WriteLine("----------");
        Console.WriteLine("Persons:");
        Console.WriteLine(dummyList.CreateTable());
    }

    private static void SingleClassEntryTest()
    {
        var entry = CreatePersonList().FirstOrDefault();

        Console.WriteLine("Single class entry");
        Console.WriteLine("------------------");
        Console.WriteLine("Person:");
        Console.WriteLine(entry!.CreateValueList());
    }

    private static void AnonymousTest()
    {
        var list = CreatePersonList().Select(s => new
        {
            s.Id,
            s.Name,
            s.Title,
            s.DepartmentId,
            s.Locked
        }).ToList();

        Console.WriteLine("Anonymous list");
        Console.WriteLine("--------------");
        Console.WriteLine("Persons:");
        Console.WriteLine(list.CreateTable());
    }

    private static void SingleAnonymousEntryTest()
    {
        var entry = CreatePersonList().Select(s => new
        {
            s.Id,
            s.Name,
            s.Title,
            s.DepartmentId,
            s.Locked
        }).FirstOrDefault();

        Console.WriteLine("Single anonymous entry test");
        Console.WriteLine("---------------------------");
        Console.WriteLine("Person:");
        Console.WriteLine(entry!.CreateValueTable());
    }

    private static List<Person> CreatePersonList()
    {
        return new List<Person>
        {
            new()
            {
                Id = 1,
                Name = "Philip J. Fry",
                Title = "",
                DepartmentId = 1,
                Locked = false
            },
            new()
            {
                Id = 2,
                Name = "Huber Farnsworth",
                Title = "Dr.",
                DepartmentId = 2,
                Locked = false
            },
            new()
            {
                Id = 3,
                Name = "John D. Zoidberg",
                Title = "Dr.",
                DepartmentId = 3,
                Locked = false
            },
            new()
            {
                Id = 4,
                Name = "Bender B. Rodriguez",
                Title = "",
                DepartmentId = 1,
                Locked = false
            }
        };
    }
}

internal class Person
{
    [Appearance(Order = 1, TextAlign = TextAlign.Right)]
    public int Id { get; set; }

    [Appearance(Order = 3)]
    public string Name { get; set; } = string.Empty;

    [Appearance(Order = 2)]
    public string Title { get; set; } = string.Empty;

    [Appearance(Ignore = true)]
    public int DepartmentId { get; set; }

    [Appearance(Name = "User locked", Order = 4)]
    public bool Locked { get; set; }
}