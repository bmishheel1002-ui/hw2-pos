using Pos.Domain;

namespace Pos.Application.Ports;

/// <summary>
/// An outgoing port for storing students.
/// </summary>
public interface IStudentRepository
{
    IReadOnlyList<Student> List();
    Student? GetById(Guid id);
    void Save(Student student);
}
