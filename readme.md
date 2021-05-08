# ZimLabs.TableCreator

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/InvaderZim85/ZimLabs.TableCreator)](https://github.com/InvaderZim85/ZimLabs.TableCreator/releases) [![Nuget](https://img.shields.io/nuget/v/ZimLabs.TableCreator)](https://www.nuget.org/packages/ZimLabs.TableCreator/)

This library is not very special :) It takes a list of objects and creates an ASCII "table", markdown table or a CSV list. Very simple and straigt forward.

## Install

```
PM > Install-Package ZimLabs.TableCreator
```

## Usage

Here a short example

```csharp
public sealed class Person
{
    // Align the text to the right
    [Appearance(TextAlign = TextAlign.Right)]
    public int Id { get; set; }

    // Set the name of the column to "First name"
    [Appearance(Name = "First name")]
    public string Name { get; set; }

    public string LastName { get; set; }

    public string Mail { get; set; }

    public string Gender { get; set; }

    [Appearance(Ignore = true)]
    public string JobTitle { get; set; }

    // Change the format of the DateTime value
    [Appearance(Format = "yyyy-MM-dd")]
    public DateTime Birthday { get; set; }
}

// Here a list with some persons...
var tempList = new List<Person>();

// Create a ASCII styled table with and show the row numbers
var result = TableCreator.CreateTable(tempList, OutputType.Default, true);
```

And the result looks like this:

```
+-----+----+------------+-------------+-------------------------------+--------+------------+
| Row | Id | First name | Last name   | E-Mail                        | Gender | Birthday   |
+-----+----+------------+-------------+-------------------------------+--------+------------+
|   1 |  1 | Tommy      | Giblin      | tgiblin0@amazon.co.uk         | Female | 1968-03-26 |
|   2 |  2 | Sven       | Puden       | spuden1@soundcloud.com        | Male   | 1952-04-24 |
|   3 |  3 | Garvy      | Czaple      | gczaple2@com.com              | Male   | 1965-04-10 |
|   4 |  4 | Eryn       | Mariotte    | emariotte3@issuu.com          | Female | 1986-07-23 |
|   5 |  5 | Zaccaria   | Oiseau      | zoiseau4@huffingtonpost.com   | Male   | 1967-11-09 |
|   6 |  6 | Conny      | Di Batista  | cdibatista5@mysql.com         | Male   | 1997-12-27 |
|   7 |  7 | Toma       | Tristram    | ttristram6@mashable.com       | Female | 1960-02-15 |
|   8 |  8 | Boniface   | Sperry      | bsperry7@behance.net          | Male   | 1985-05-13 |
|   9 |  9 | Nevins     | Stear       | nstear8@aboutads.info         | Male   | 1951-04-05 |
|  10 | 10 | Yolane     | Wadman      | ywadman9@stanford.edu         | Female | 1962-05-28 |
+-----+----+------------+-------------+-------------------------------+--------+------------+
```
