namespace Pos.Domain;

/// <summary>
/// Creates evaluations after checking the student, criterion, and score.
/// </summary>
public sealed class EvaluationFactory
{
    public Evaluation Create(
        Guid id,
        Student student,
        Criterion criterion,
        decimal points,
        string? comment = null)
    {
        ArgumentNullException.ThrowIfNull(student);
        ArgumentNullException.ThrowIfNull(criterion);

        var score = new Score(points, criterion.MaximumScore);
        return new Evaluation(
            id,
            student.Id,
            criterion.Id,
            score,
            comment ?? string.Empty,
            DateTimeOffset.UtcNow);
    }
}
