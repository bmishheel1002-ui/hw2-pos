using Pos.Domain;

namespace Pos.Application;

public sealed record StudentGradeReport(
    Student Student,
    FinalGrade? FinalGrade,
    int EvaluatedCriteriaCount,
    int TotalCriteriaCount);
