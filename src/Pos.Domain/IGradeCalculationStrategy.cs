namespace Pos.Domain;

/// <summary>
/// Defines how current evaluations become a final grade.
/// </summary>
public interface IGradeCalculationStrategy
{
    FinalGrade Calculate(IReadOnlyList<Evaluation> evaluations, IReadOnlyList<Criterion> rubric);
}
