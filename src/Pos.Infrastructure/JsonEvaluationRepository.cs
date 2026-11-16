using Pos.Application.Ports;
using Pos.Domain;

namespace Pos.Infrastructure;

public sealed class JsonEvaluationRepository : IEvaluationRepository
{
    private readonly JsonFileStore<EvaluationData> _store;
    private readonly List<EvaluationData> _evaluations;

    public JsonEvaluationRepository(string filePath = "data/evaluations.json")
    {
        _store = new JsonFileStore<EvaluationData>(filePath);
        _evaluations = _store.Load();
    }

    public IReadOnlyList<Evaluation> ListByStudent(Guid studentId) =>
        _evaluations.Where(item => item.StudentId == studentId).Select(ToDomain).ToList();

    public Evaluation? GetByStudentAndCriterion(Guid studentId, Guid criterionId)
    {
        var data = _evaluations.FirstOrDefault(item =>
            item.StudentId == studentId && item.CriterionId == criterionId);
        return data is null ? null : ToDomain(data);
    }

    public void Save(Evaluation evaluation)
    {
        var data = _evaluations.FirstOrDefault(item => item.Id == evaluation.Id);
        var evaluationData = new EvaluationData
        {
            Id = evaluation.Id,
            StudentId = evaluation.StudentId,
            CriterionId = evaluation.CriterionId,
            Points = evaluation.Score.Points,
            MaximumPoints = evaluation.Score.MaximumPoints,
            Comment = evaluation.Comment,
            EvaluatedAt = evaluation.EvaluatedAt
        };

        if (data is null)
            _evaluations.Add(evaluationData);
        else
            _evaluations[_evaluations.IndexOf(data)] = evaluationData;

        _store.Save(_evaluations);
    }

    private static Evaluation ToDomain(EvaluationData data)
    {
        var score = new Score(data.Points, data.MaximumPoints);
        return new Evaluation(
            data.Id,
            data.StudentId,
            data.CriterionId,
            score,
            data.Comment,
            data.EvaluatedAt);
    }

    private sealed class EvaluationData
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public Guid CriterionId { get; set; }
        public decimal Points { get; set; }
        public decimal MaximumPoints { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTimeOffset EvaluatedAt { get; set; }
    }
}
