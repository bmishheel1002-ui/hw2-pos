namespace Pos.Domain;

/// <summary>
/// A domain service that defers the final-grade rule to a strategy.
/// </summary>
public sealed class FinalGradeCalculator(IGradeCalculationStrategy strategy)
{
    private readonly IGradeCalculationStrategy _strategy = strategy;

    public FinalGrade Calculate(IReadOnlyList<Evaluation> evaluations, IReadOnlyList<Criterion> rubric)
    {
        return _strategy.Calculate(evaluations, rubric);
    }
}
