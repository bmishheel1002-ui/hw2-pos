using Pos.Application.Ports;
using Pos.Domain;

namespace Pos.Application;

/// <summary>
/// Builds current grade reports from students, the rubric, and evaluations.
/// </summary>
public sealed class GradeReportService(
    IStudentRepository studentRepository,
    IRubricRepository rubricRepository,
    IEvaluationRepository evaluationRepository,
    FinalGradeCalculator calculator,
    IGradeExporter exporter)
{
    public StudentGradeReport GetForStudent(Guid studentId)
    {
        var student = studentRepository.GetById(studentId)
            ?? throw new InvalidOperationException("Student was not found.");
        var rubric = rubricRepository.List();
        var evaluations = evaluationRepository.ListByStudent(studentId);
        var grade = evaluations.Count == 0 ? null : calculator.Calculate(evaluations, rubric);

        return new StudentGradeReport(student, grade, evaluations.Count, rubric.Count);
    }

    public IReadOnlyList<StudentGradeReport> GetForAllStudents()
    {
        return studentRepository.List().Select(student => GetForStudent(student.Id)).ToList();
    }

    public string ExportAll() => exporter.Export(GetForAllStudents());
}
