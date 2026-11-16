using Pos.Application;
using Pos.Application.Ports;
using Pos.Domain;

namespace Pos.Tests;

public sealed class PosTests
{
    [Fact]
    public void Score_rejects_points_above_the_criterion_maximum()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Score(11, 10));
    }

    [Fact]
    public void Criterion_rejects_a_zero_weight()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Criterion(Guid.NewGuid(), "Quiz", 10, 0));
    }

    [Fact]
    public void Weighted_average_uses_each_criterion_weight()
    {
        var student = new Student(Guid.NewGuid(), "Misheel");
        var first = new Criterion(Guid.NewGuid(), "Quiz", 5, 1);
        var second = new Criterion(Guid.NewGuid(), "Project", 10, 3);
        var factory = new EvaluationFactory();
        var evaluations = new[]
        {
            factory.Create(Guid.NewGuid(), student, first, 4),
            factory.Create(Guid.NewGuid(), student, second, 5)
        };

        var grade = new WeightedAverageGradeStrategy().Calculate(evaluations, [first, second]);

        Assert.Equal(5.75m, grade.Points);
    }

    [Fact]
    public void Missing_score_strategy_counts_ungraded_criteria_as_zero()
    {
        var student = new Student(Guid.NewGuid(), "Misheel");
        var completed = new Criterion(Guid.NewGuid(), "Quiz", 10, 1);
        var notCompleted = new Criterion(Guid.NewGuid(), "Project", 20, 3);
        var evaluation = new EvaluationFactory().Create(Guid.NewGuid(), student, completed, 10);

        var grade = new WeightedAverageIncludingMissingAsZeroStrategy()
            .Calculate([evaluation], [completed, notCompleted]);

        Assert.Equal(2.5m, grade.Points);
    }

    [Fact]
    public void Calculation_rejects_a_criterion_outside_the_rubric()
    {
        var student = new Student(Guid.NewGuid(), "Misheel");
        var criterion = new Criterion(Guid.NewGuid(), "Quiz", 10, 1);
        var evaluation = new EvaluationFactory().Create(Guid.NewGuid(), student, criterion, 8);

        Assert.Throws<InvalidOperationException>(() =>
            new WeightedAverageGradeStrategy().Calculate([evaluation], []));
    }

    [Fact]
    public void Evaluation_service_rejects_a_missing_student()
    {
        var repository = new MemoryEvaluationRepository();
        var service = CreateEvaluationService(new MemoryStudentRepository(), new MemoryRubricRepository(), repository);

        Assert.Throws<InvalidOperationException>(() =>
            service.SetScore(Guid.NewGuid(), Guid.NewGuid(), 5, null));
        Assert.Empty(repository.Items);
    }

    [Fact]
    public void Evaluation_service_rejects_a_criterion_outside_the_rubric()
    {
        var student = new Student(Guid.NewGuid(), "Misheel");
        var students = new MemoryStudentRepository();
        students.Save(student);
        var repository = new MemoryEvaluationRepository();
        var service = CreateEvaluationService(students, new MemoryRubricRepository(), repository);

        Assert.Throws<InvalidOperationException>(() =>
            service.SetScore(student.Id, Guid.NewGuid(), 5, null));
        Assert.Empty(repository.Items);
    }

    [Fact]
    public void Setting_a_score_again_updates_the_existing_evaluation()
    {
        var student = new Student(Guid.NewGuid(), "Misheel");
        var criterion = new Criterion(Guid.NewGuid(), "Quiz", 10, 1);
        var students = new MemoryStudentRepository();
        var rubric = new MemoryRubricRepository();
        var evaluations = new MemoryEvaluationRepository();
        students.Save(student);
        rubric.Save(criterion);
        var service = CreateEvaluationService(students, rubric, evaluations);

        var first = service.SetScore(student.Id, criterion.Id, 4, "Draft");
        var updated = service.SetScore(student.Id, criterion.Id, 8, "Final");

        Assert.Equal(first.Id, updated.Id);
        Assert.Single(evaluations.Items);
        Assert.Equal(8, updated.Score.Points);
        Assert.Equal("Final", updated.Comment);
    }

    [Fact]
    public void Rubric_service_rejects_duplicate_criterion_names()
    {
        var rubric = new MemoryRubricRepository();
        var service = new RubricService(rubric);
        service.Add("Project", 10, 2);

        Assert.Throws<InvalidOperationException>(() => service.Add("project", 5, 1));
    }

    [Fact]
    public void Grade_report_shows_no_final_grade_before_the_first_evaluation()
    {
        var student = new Student(Guid.NewGuid(), "Misheel");
        var students = new MemoryStudentRepository();
        students.Save(student);
        var reportService = new GradeReportService(
            students,
            new MemoryRubricRepository(),
            new MemoryEvaluationRepository(),
            new FinalGradeCalculator(new WeightedAverageGradeStrategy()),
            new MemoryGradeExporter());

        var report = reportService.GetForStudent(student.Id);

        Assert.Null(report.FinalGrade);
        Assert.Equal(0, report.EvaluatedCriteriaCount);
    }

    private static EvaluationService CreateEvaluationService(
        MemoryStudentRepository students,
        MemoryRubricRepository rubric,
        MemoryEvaluationRepository evaluations)
    {
        return new EvaluationService(students, rubric, evaluations, new EvaluationFactory());
    }

    private sealed class MemoryStudentRepository : IStudentRepository
    {
        private readonly List<Student> _students = [];
        public IReadOnlyList<Student> List() => _students.ToList();
        public Student? GetById(Guid id) => _students.FirstOrDefault(student => student.Id == id);
        public void Save(Student student) => _students.Add(student);
    }

    private sealed class MemoryRubricRepository : IRubricRepository
    {
        private readonly List<Criterion> _criteria = [];
        public IReadOnlyList<Criterion> List() => _criteria.ToList();
        public Criterion? GetById(Guid id) => _criteria.FirstOrDefault(criterion => criterion.Id == id);
        public void Save(Criterion criterion) => _criteria.Add(criterion);
    }

    private sealed class MemoryEvaluationRepository : IEvaluationRepository
    {
        public List<Evaluation> Items { get; } = [];
        public IReadOnlyList<Evaluation> ListByStudent(Guid studentId) =>
            Items.Where(item => item.StudentId == studentId).ToList();
        public Evaluation? GetByStudentAndCriterion(Guid studentId, Guid criterionId) =>
            Items.FirstOrDefault(item => item.StudentId == studentId && item.CriterionId == criterionId);
        public void Save(Evaluation evaluation)
        {
            var index = Items.FindIndex(item => item.Id == evaluation.Id);
            if (index < 0)
                Items.Add(evaluation);
            else
                Items[index] = evaluation;
        }
    }

    private sealed class MemoryGradeExporter : IGradeExporter
    {
        public string Export(IReadOnlyList<StudentGradeReport> reports) => "memory.csv";
    }
}
