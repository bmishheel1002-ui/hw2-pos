using Pos.Application.Ports;
using Pos.Domain;

namespace Pos.Application;

/// <summary>
/// Runs rubric-related application use cases.
/// </summary>
public sealed class RubricService(IRubricRepository repository)
{
    public Criterion Add(string name, decimal maximumScore, decimal weight)
    {
        if (repository.List().Any(criterion =>
                string.Equals(criterion.Name, name.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidOperationException("A criterion with this name already exists.");
        }

        var criterion = new Criterion(Guid.NewGuid(), name, maximumScore, weight);
        repository.Save(criterion);
        return criterion;
    }

    public IReadOnlyList<Criterion> List() => repository.List();
}
