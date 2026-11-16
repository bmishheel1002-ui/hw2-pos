using Pos.Domain;

namespace Pos.Application.Ports;

/// <summary>
/// An outgoing port for storing rubric criteria.
/// </summary>
public interface IRubricRepository
{
    IReadOnlyList<Criterion> List();
    Criterion? GetById(Guid id);
    void Save(Criterion criterion);
}
