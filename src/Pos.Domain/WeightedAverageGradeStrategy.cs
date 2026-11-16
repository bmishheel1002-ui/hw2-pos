namespace Pos.Domain;

/// <summary>
/// Calculates a weighted average on a ten-point scale.
/// </summary>
public sealed class WeightedAverageGradeStrategy : IGradeCalculationStrategy
{
    public FinalGrade Calculate(IReadOnlyList<Evaluation> evaluations, IReadOnlyList<Criterion> rubric)
    {
        ArgumentNullException.ThrowIfNull(evaluations);
        ArgumentNullException.ThrowIfNull(rubric);

        if (evaluations.Count == 0)
            throw new InvalidOperationException("A final grade needs at least one evaluation.");

        var criteriaById = rubric.ToDictionary(criterion => criterion.Id);
        var seenCriterionIds = new HashSet<Guid>();
        decimal weightedScore = 0;
        decimal totalWeight = 0;

        foreach (var evaluation in evaluations)
        {
            if (!criteriaById.TryGetValue(evaluation.CriterionId, out var criterion))
                throw new InvalidOperationException("An evaluation refers to a criterion outside the rubric.");

            if (!seenCriterionIds.Add(evaluation.CriterionId))
                throw new InvalidOperationException("A student can have only one current evaluation per criterion.");

            if (evaluation.Score.MaximumPoints != criterion.MaximumScore)
                throw new InvalidOperationException("The evaluation scale does not match the rubric criterion.");

            weightedScore += evaluation.Score.Points / criterion.MaximumScore * criterion.Weight;
            totalWeight += criterion.Weight;
        }

        return new FinalGrade(weightedScore / totalWeight * 10);
    }
}
