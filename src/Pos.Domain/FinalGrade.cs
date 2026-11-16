namespace Pos.Domain;

/// <summary>
/// A value object for a final grade on a ten-point scale.
/// </summary>
public sealed record FinalGrade
{
    public decimal Points { get; }

    public FinalGrade(decimal points)
    {
        if (points < 0 || points > 10)
            throw new ArgumentOutOfRangeException(nameof(points), "A final grade must be between zero and ten.");

        Points = decimal.Round(points, 2, MidpointRounding.AwayFromZero);
    }
}
