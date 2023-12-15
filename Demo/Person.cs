using ZimLabs.TableCreator;

namespace Demo;

internal sealed class Person
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

    [Appearance(Format = "N0", Order = 5)]
    public int SomeValue { get; set; }

    [Appearance(Format = "yyyy-MM-dd", Order = 6)]
    public DateTime SomeDate { get; set; }

    [Appearance(Order = 7)]
    public string? SomeNullableValue { get; set; }
}