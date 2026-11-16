using Pos.Domain;

namespace Pos.Application;

/// <summary>
/// A facade that gives the console one simple entry point to POS use cases.
/// </summary>
public sealed class PosFacade(
    StudentService students,
    RubricService rubric,
    EvaluationService evaluations,
    GradeReportService reports)
{
    public Student AddStudent(string name) => students.Add(name);
    public IReadOnlyList<Student> ListStudents() => students.List();
    public Criterion AddCriterion(string name, decimal maximumScore, decimal weight) =>
        rubric.Add(name, maximumScore, weight);
    public IReadOnlyList<Criterion> ListCriteria() => rubric.List();
    public Evaluation SetScore(Guid studentId, Guid criterionId, decimal points, string? comment = null) =>
        evaluations.SetScore(studentId, criterionId, points, comment);
    public IReadOnlyList<Evaluation> ListEvaluations(Guid studentId) => evaluations.ListForStudent(studentId);
    public StudentGradeReport GetReport(Guid studentId) => reports.GetForStudent(studentId);
    public IReadOnlyList<StudentGradeReport> GetAllReports() => reports.GetForAllStudents();
    public string ExportFinalGrades() => reports.ExportAll();
}
