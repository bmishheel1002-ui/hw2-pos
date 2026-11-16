using Pos.Application.Ports;
using Pos.Domain;

namespace Pos.Infrastructure;

/// <summary>
/// Stores students in a JSON file.
/// </summary>
public sealed class JsonStudentRepository : IStudentRepository
{
    private readonly JsonFileStore<StudentData> _store;
    private readonly List<StudentData> _students;

    public JsonStudentRepository(string filePath = "data/students.json")
    {
        _store = new JsonFileStore<StudentData>(filePath);
        _students = _store.Load();
    }

    public IReadOnlyList<Student> List() => _students.Select(ToDomain).ToList();

    public Student? GetById(Guid id)
    {
        var data = _students.FirstOrDefault(student => student.Id == id);
        return data is null ? null : ToDomain(data);
    }

    public void Save(Student student)
    {
        var data = _students.FirstOrDefault(item => item.Id == student.Id);
        var studentData = new StudentData { Id = student.Id, Name = student.Name };

        if (data is null)
            _students.Add(studentData);
        else
            _students[_students.IndexOf(data)] = studentData;

        _store.Save(_students);
    }

    private static Student ToDomain(StudentData data) => new(data.Id, data.Name);

    private sealed class StudentData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
