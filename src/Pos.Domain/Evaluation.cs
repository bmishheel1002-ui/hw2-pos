namespace Pos.Domain;

/// <summary>
/// An entity that records a student's score for one rubric criterion.
/// </summary>
public sealed class Evaluation
{
    public Guid Id { get; }
    public Guid StudentId { get; }
    public Guid CriterionId { get; }
    public Score Score { get; }
    public string Comment { get; }
    public DateTimeOffset EvaluatedAt { get; }

    public Evaluation(
        Guid id,
        Guid studentId,
        Guid criterionId,
        Score score,
        string comment,
        DateTimeOffset evaluatedAt)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Evaluation ID cannot be empty.", nameof(id));
        if (studentId == Guid.Empty)
            throw new ArgumentException("Student ID cannot be empty.", nameof(studentId));
        if (criterionId == Guid.Empty)
            throw new ArgumentException("Criterion ID cannot be empty.", nameof(criterionId));

        Id = id;
        StudentId = studentId;
        CriterionId = criterionId;
        Score = score ?? throw new ArgumentNullException(nameof(score));
        Comment = comment.Trim();
        EvaluatedAt = evaluatedAt;
    }
}
