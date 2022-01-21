# ZimLabs.TableCreator

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/InvaderZim85/ZimLabs.TableCreator)](https://github.com/InvaderZim85/ZimLabs.TableCreator/releases) [![Nuget](https://img.shields.io/nuget/v/ZimLabs.TableCreator)](https://www.nuget.org/packages/ZimLabs.TableCreator/)

This library is not very special :) It takes a list of objects and creates an ASCII "table", markdown table or a CSV list. Very simple and straight forward.

---

**Content**

<!-- TOC -->

- [Install](#install)
- [Usage](#usage)
- [Known issues](#known-issues)

<!-- /TOC -->

## Install

```
PM > Install-Package ZimLabs.TableCreator
```

## Usage

Attributes:

```csharp
internal sealed class Person
{
    [Appearance(TextAlign = TextAlign.Right, Order = 1)]
    public int Id { get; set; }

    [Appearance(Name = "First name", Order = 3)]
    public string Name { get; set; }

    [Appearance(Order = 2)]
    public string LastName { get; set; }

    [Appearance(Name = "E-Mail", Order = 4)]
    public string Mail { get; set; }

    [Appearance(Ignore = true)]
    public string Gender { get; set; }

    [Appearance(Name = "Job title")]
    public string JobTitle { get; set; }

    [Appearance(Format = "yyyy-MM-dd")]
    public DateTime Birthday { get; set; }
}
```

> **Note**: If not all properties have an Order value, the following order is applied:

1. occurrence in the class
2. order according to Order value

Usage of the Create / Save methods:

```csharp
var personList = CreateDummyList().ToList();

// Print the complete list
Console.WriteLine("Person List");
Console.WriteLine(personList.CreateTable());

// Save the person list
personList.SaveTable("PersonList.txt");

// Print a single person
var person = personList.FirstOrDefault();

Console.WriteLine("Single person");
Console.WriteLine("Value list");
Console.WriteLine(person.CreateValueList());

Console.WriteLine("Value table");
Console.WriteLine(person.CreateValueTable());

// Save the person (as value list)
person.SaveValue("FileName.txt");

// Save the person (as table)
person.SaveValueAsTable("FileName.txt");

Console.WriteLine("Done");
Console.ReadLine();
```

The result:

```
Person List
+-------------------------------+------------+----+-------------+------------+-------------------------------+
| Job title                     | Birthday   | Id | LastName    | First name | E-Mail                        |
+-------------------------------+------------+----+-------------+------------+-------------------------------+
| Environmental Tech            | 1968-03-26 |  1 | Giblin      | Tommy      | tgiblin0@amazon.co.uk         |
| Teacher                       | 1952-04-24 |  2 | Puden       | Sven       | spuden1@soundcloud.com        |
| VP Quality Control            | 1965-04-10 |  3 | Czaple      | Garvy      | gczaple2@com.com              |
| Pharmacist                    | 1986-07-23 |  4 | Mariotte    | Eryn       | emariotte3@issuu.com          |
| Senior Financial Analyst      | 1967-11-09 |  5 | Oiseau      | Zaccaria   | zoiseau4@huffingtonpost.com   |
+-------------------------------+------------+----+-------------+------------+-------------------------------+

Single person
Value list
- Job title.: Environmental Tech
- Birthday..: 26/03/1968 00:00:00
- Id........: 1
- LastName..: Giblin
- First name: Tommy
- E-Mail....: tgiblin0@amazon.co.uk

Value table
+------------+-----------------------+
| Key        | Value                 |
+------------+-----------------------+
| Job title  | Environmental Tech    |
| Birthday   | 26/03/1968 00:00:00   |
| Id         | 1                     |
| LastName   | Giblin                |
| First name | Tommy                 |
| E-Mail     | tgiblin0@amazon.co.uk |
+------------+-----------------------+
```

## Known issues

Currently it's possible to call the methods for a single entry with a list:

```csharp
// Wrong call
personList.CreateValueList();
```

This call will cause a `System.Reflection.TargetParameterCountException`.

I'll try to fix the bug in the next version (1.4).
