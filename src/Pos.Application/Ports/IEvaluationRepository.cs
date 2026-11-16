using Pos.Domain;

namespace Pos.Application.Ports;

/// <summary>
/// An outgoing port for storing current student evaluations.
/// </summary>
public interface IEvaluationRepository
{
    IReadOnlyList<Evaluation> ListByStudent(Guid studentId);
    Evaluation? GetByStudentAndCriterion(Guid studentId, Guid criterionId);
    void Save(Evaluation evaluation);
}
