namespace Pos.Domain;

/// <summary>
/// An entity representing a student who can receive evaluations.
/// </summary>
public sealed class Student
{
    public Guid Id { get; }
    public string Name { get; }

    public Student(Guid id, string name)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Student name cannot be empty.", nameof(name));

        Id = id;
        Name = name.Trim();
    }
}
