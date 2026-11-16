namespace Pos.Domain;

/// <summary>
/// Calculates a weighted average and treats ungraded rubric criteria as zero.
/// </summary>
public sealed class WeightedAverageIncludingMissingAsZeroStrategy : IGradeCalculationStrategy
{
    public FinalGrade Calculate(IReadOnlyList<Evaluation> evaluations, IReadOnlyList<Criterion> rubric)
    {
        ArgumentNullException.ThrowIfNull(evaluations);
        ArgumentNullException.ThrowIfNull(rubric);

        if (rubric.Count == 0)
            throw new InvalidOperationException("A final grade needs at least one rubric criterion.");

        var criteriaById = rubric.ToDictionary(criterion => criterion.Id);
        var evaluationsByCriterion = new Dictionary<Guid, Evaluation>();

        foreach (var evaluation in evaluations)
        {
            if (!criteriaById.TryGetValue(evaluation.CriterionId, out var criterion))
                throw new InvalidOperationException("An evaluation refers to a criterion outside the rubric.");

            if (evaluation.Score.MaximumPoints != criterion.MaximumScore)
                throw new InvalidOperationException("The evaluation scale does not match the rubric criterion.");

            if (!evaluationsByCriterion.TryAdd(evaluation.CriterionId, evaluation))
                throw new InvalidOperationException("A student can have only one current evaluation per criterion.");
        }

        var weightedScore = rubric.Sum(criterion =>
        {
            if (!evaluationsByCriterion.TryGetValue(criterion.Id, out var evaluation))
                return 0;

            return evaluation.Score.Points / criterion.MaximumScore * criterion.Weight;
        });

        var totalWeight = rubric.Sum(criterion => criterion.Weight);
        return new FinalGrade(weightedScore / totalWeight * 10);
    }
}
