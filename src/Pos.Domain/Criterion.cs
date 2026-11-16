namespace Pos.Domain;

/// <summary>
/// An entity that defines one part of the course rubric.
/// </summary>
public sealed class Criterion
{
    public Guid Id { get; }
    public string Name { get; }
    public decimal MaximumScore { get; }
    public decimal Weight { get; }

    public Criterion(Guid id, string name, decimal maximumScore, decimal weight)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Criterion ID cannot be empty.", nameof(id));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Criterion name cannot be empty.", nameof(name));

        if (maximumScore <= 0)
            throw new ArgumentOutOfRangeException(nameof(maximumScore), "Maximum score must be greater than zero.");

        if (weight <= 0)
            throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be greater than zero.");

        Id = id;
        Name = name.Trim();
        MaximumScore = maximumScore;
        Weight = weight;
    }
}
