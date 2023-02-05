using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using ZimLabs.TableCreator;
using ZimLabs.TableCreator.DataObjects;

namespace Demo
{
    internal static class Program
    {
        private static void Main()
        {
            var data = CreateDummyList().OrderByDescending(o => o.Id).ToList();

            //Console.WriteLine(data.CreateTable());

            var person = data.FirstOrDefault(f => f.SomeNumber > 10000);

            var overrideList = new List<OverrideAttributeEntry>
            {
                new("Name", new AppearanceAttribute
                {
                    Name = "Some other Name"
                }),
                new("SomeNumber", new AppearanceAttribute
                {
                    Format = "N2"
                })
            };

            Console.WriteLine(person.CreateValueTable(overrideList: overrideList));
            Console.WriteLine(person.CreateValueList());

            // Save as file
            person.SaveValueAsTable("TestFile.txt");

            Console.WriteLine("");
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        private static List<Person> CreateDummyList()
        {
            var filePath = Path.GetFullPath("MOCK_DATA.json");

            var content = File.ReadAllText(filePath, Encoding.UTF8);

            var list = JsonConvert.DeserializeObject<List<Person>>(content);

            var rnd = new Random();
            foreach (var entry in list)
            {
                entry.SomeNumber = rnd.Next(0, int.MaxValue);
            }

            return list;
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

        [Appearance(Ignore = true)]
        public string Gender { get; set; }

        public string JobTitle { get; set; }

        [Appearance(Format = "N0")]
        public int SomeNumber { get; set; }

        [Appearance(Format = "yyyy-MM-dd")]
        public DateTime Birthday { get; set; }
    }
}
