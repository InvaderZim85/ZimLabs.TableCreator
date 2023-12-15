using System.Data;

namespace Demo;

/// <summary>
/// Provides several helper functions
/// </summary>
internal sealed class Helper
{
    public static Person CreatePersonEntry()
    {
        var random = new Random();
        return new Person
        {
            Id = 1,
            Name = "Philip J. Fry",
            Title = "",
            DepartmentId = 1,
            Locked = false,
            SomeValue = random.Next(1000, 10000),
            SomeDate = DateTime.Now.AddDays(random.Next(1, 30))
        };
    }

    public static List<Person> CreatePersonList()
    {
        var random = new Random();
        return
        [
            new Person
            {
                Id = 1,
                Name = "Philip J. Fry",
                Title = "",
                DepartmentId = 1,
                Locked = false,
                SomeValue = random.Next(1000, 10000),
                SomeDate = DateTime.Now.AddDays(random.Next(1, 30))
            },

            new Person
            {
                Id = 2,
                Name = "Huber Farnsworth",
                Title = "Dr.",
                DepartmentId = 2,
                Locked = false,
                SomeValue = random.Next(1000, 10000),
                SomeDate = DateTime.Now.AddDays(random.Next(1, 30))
            },

            new Person
            {
                Id = 3,
                Name = "John D. Zoidberg",
                Title = "Dr.",
                DepartmentId = 3,
                Locked = false,
                SomeValue = random.Next(1000, 10000),
                SomeDate = DateTime.Now.AddDays(random.Next(1, 30))
            },

            new Person
            {
                Id = 4,
                Name = "Bender B. Rodriguez",
                Title = "",
                DepartmentId = 1,
                Locked = false,
                SomeValue = random.Next(1000, 10000),
                SomeDate = DateTime.Now.AddDays(random.Next(1, 30))
            }
        ];
    }

    public static DataTable CreatePersonTable()
    {
        // Create a table
        var dataTable = new DataTable("Persons");
        dataTable.Columns.Add("Id", typeof(int));
        dataTable.Columns.Add("Name", typeof(string));
        dataTable.Columns.Add("Title", typeof(string));
        dataTable.Columns.Add("DepartmentId", typeof(int));
        dataTable.Columns.Add("Locked", typeof(bool));
        dataTable.Columns.Add("SomeValue", typeof(int));
        dataTable.Columns.Add("SomeDate", typeof(DateTime));

        // Add some rows...
        var personList = CreatePersonList();

        foreach (var person in personList)
        {
            dataTable.Rows.Add(person.Id,
                person.Name,
                person.Title,
                person.DepartmentId,
                person.Locked,
                person.SomeValue,
                person.SomeDate);
        }

        return dataTable;
    }
}