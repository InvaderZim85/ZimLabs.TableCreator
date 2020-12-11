using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ZimLabs.TableCreator;

namespace Demo
{
    internal static class Program
    {
        private static void Main()
        {
            var data = CreateDummyList();

            var tableString = TableCreator.CreateTable(data, OutputType.Default, true);

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
