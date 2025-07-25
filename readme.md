# ZimLabs.TableCreator

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/InvaderZim85/ZimLabs.TableCreator)](https://github.com/InvaderZim85/ZimLabs.TableCreator/releases) [![Nuget](https://img.shields.io/nuget/v/ZimLabs.TableCreator)](https://www.nuget.org/packages/ZimLabs.TableCreator/)

This library is not very special :) It takes a list of objects and creates an ASCII "table", markdown table or a CSV list. Very simple and straight forward.

---

**Content**

<!-- TOC -->

- [Install](#install)
- [Usage](#usage)
- [Known issues](#known-issues)
- [Changelog](#changelog)
    - [Version 3.1.0](#version-310)
    - [Version 3.0.0](#version-300)
    - [Version 2.1.1](#version-211)
    - [Version 2.1.0](#version-210)
    - [Version 2.0.2](#version-202)
    - [Version 2.0.1](#version-201)
    - [Version 2.0.0](#version-200)
    - [Version 1.5.0](#version-150)

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

For more examples take a look at the demo project.

## Known issues

Currently it's possible to call the methods for a single entry with a list:

```csharp
// Wrong call
personList.CreateValueList();
```

~~I'll try to fix the bug in the next version (1.5).~~

Since I didn't find a solution to prevent calling the method for a single value, I included a check that throws an error when trying to call the method for a single value from a list.

If you call from a list a function, which was developed only for a single value, an `NotSupportedException` with the following message will be thrown:

```
The specified type is not supported by this method. Please choose "CreateTable" or "SaveTable" instead.
```

Sorry for the inconvenience.

## Changelog

### Version 3.1.0

Removed the old *obsolete* methods (for more information see [Version 3.0.0](#version-300))

### Version 3.0.0

🚨🚨🚨 BREAKING CHANGE 🚨🚨🚨

Between the last version and this one the runtime environment has been changed! New runtime environment is **.NET 9**

Other changes:

- Code tidied up
- Property `EncapsulateText` added. If the value is set to `true`, every text value is automatically set in quotation marks, regardless of which attribute is set for the corresponding property.

**IMPORTANT**

Several methods have been given the obsolete attribute. These methods will be removed in the next version!

Why? The options have been moved to a separate class. This streamlines the actual code, as the other methods only functioned as "wrappers". The new options class also makes it easier to use, as you can see at a glance which options have been set.

### Version 2.1.1

- Option `addHeader` addded, with which you can decide whether the CSV content should contain a header line.
- Added classes for the various options to improve clarity (`TableCreateOptions` and `TableCreatorListOptions`).
- Appearance attribute adjusted. It is now possible to specify whether the content of a property should be encapsulate in quotation marks.

### Version 2.1.0

- Fixed some bugs:
    - Custom *format* was not used in some cases (`DataTable` functions)
    - Missing `null` check in the list functions
- Added .NET 8 support (multiple target frameworks). Now .NET 7 and .NET 8 are supported
- Minor changes under the hood (usage of the new C# 12 features)

### Version 2.0.2

- Added missing api documentation

### Version 2.0.1

- Fixed a bug in the `DataTable` CSV generator 

### Version 2.0.0

🚨🚨🚨 BREAKING CHANGE 🚨🚨🚨

Between the last version and this one the runtime environment has been changed! New runtime environment is **.NET 7**

Other changes:

- Added the support of `DataTable`

### Version 1.5.0

Added check function to the following methods

- `CreateValueTable`
- `SaveValue`
- `SaveValueAsTable`

The function checks if the given value is a list or not. If the value is a list an exception (`NotSupportedException`) will be thrown.

For more information see [Known issues](#known-issues).

I also removed the documentation for the classes, because the tool I use for that doesn't seem to cope with .NET Standard, so the documentation wasn't updated anymore.