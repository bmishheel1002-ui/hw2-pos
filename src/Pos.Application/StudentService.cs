using Pos.Application.Ports;
using Pos.Domain;

namespace Pos.Application;

/// <summary>
/// Runs student-related application use cases.
/// </summary>
public sealed class StudentService(IStudentRepository repository)
{
    public Student Add(string name)
    {
        var student = new Student(Guid.NewGuid(), name);
        repository.Save(student);
        return student;
    }

    public IReadOnlyList<Student> List() => repository.List();
}
