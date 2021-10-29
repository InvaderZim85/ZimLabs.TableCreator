using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ZimLabs.TableCreator;

namespace Demo
{
    internal static class Program
    {
        private static void Main()
        {
            var data = CreateDummyList().OrderByDescending(o => o.Id);

            var tableString = data.CreateTable(OutputType.Csv, true);

            Console.WriteLine(tableString);

            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static IEnumerable<Person> CreateDummyList()
        {
            var filePath = Path.GetFullPath("MOCK_DATA.json");

            var content = File.ReadAllText(filePath, Encoding.UTF8);

            return JsonConvert.DeserializeObject<List<Person>>(content);
        }

        public static IReadOnlyCollection<Person> CreateErrorList()
        {
            return new List<Person>
            {
                new Person
                {
                    Id = 1,
                    Name = "Andreas",
                    Mail = "Test@test.mail",
                    Birthday = new DateTime(1985, 7, 12)
                }
            };
        }
    }

    internal sealed class Person
    {
        [Appearance(TextAlign = TextAlign.Right)]
        public int Id { get; set; }

        [Appearance(Name = "First name")]
        public string Name { get; set; }

        [Appearance(Name = "Last name")]
        public string LastName { get; set; }

        [Appearance(Name = "E-Mail")]
        public string Mail { get; set; }

        public string Gender { get; set; }

        [Appearance(Ignore = true)]
        public string JobTitle { get; set; }

        [Appearance(Format = "yyyy-MM-dd")]
        public DateTime Birthday { get; set; }
    }
}
