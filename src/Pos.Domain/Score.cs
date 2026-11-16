namespace Pos.Domain;

/// <summary>
/// A value object for points given for one criterion.
/// </summary>
public sealed record Score
{
    public decimal Points { get; }
    public decimal MaximumPoints { get; }

    public Score(decimal points, decimal maximumPoints)
    {
        if (maximumPoints <= 0)
            throw new ArgumentOutOfRangeException(nameof(maximumPoints), "Maximum points must be greater than zero.");

        if (points < 0 || points > maximumPoints)
            throw new ArgumentOutOfRangeException(nameof(points), "Points must be between zero and the maximum score.");

        Points = points;
        MaximumPoints = maximumPoints;
    }
}
