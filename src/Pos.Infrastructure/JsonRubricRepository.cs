using Pos.Application.Ports;
using Pos.Domain;

namespace Pos.Infrastructure;

/// <summary>
/// Stores rubric criteria in a JSON file.
/// </summary>
public sealed class JsonRubricRepository : IRubricRepository
{
    private readonly JsonFileStore<CriterionData> _store;
    private readonly List<CriterionData> _criteria;

    public JsonRubricRepository(string filePath = "data/criteria.json")
    {
        _store = new JsonFileStore<CriterionData>(filePath);
        _criteria = _store.Load();
    }

    public IReadOnlyList<Criterion> List() => _criteria.Select(ToDomain).ToList();

    public Criterion? GetById(Guid id)
    {
        var data = _criteria.FirstOrDefault(criterion => criterion.Id == id);
        return data is null ? null : ToDomain(data);
    }

    public void Save(Criterion criterion)
    {
        var data = _criteria.FirstOrDefault(item => item.Id == criterion.Id);
        var criterionData = new CriterionData
        {
            Id = criterion.Id,
            Name = criterion.Name,
            MaximumScore = criterion.MaximumScore,
            Weight = criterion.Weight
        };

        if (data is null)
            _criteria.Add(criterionData);
        else
            _criteria[_criteria.IndexOf(data)] = criterionData;

        _store.Save(_criteria);
    }

    private static Criterion ToDomain(CriterionData data) =>
        new(data.Id, data.Name, data.MaximumScore, data.Weight);

    private sealed class CriterionData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal MaximumScore { get; set; }
        public decimal Weight { get; set; }
    }
}
