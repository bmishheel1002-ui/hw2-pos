using Pos.Application.Ports;
using Pos.Domain;

namespace Pos.Application;

/// <summary>
/// Runs the use case for creating or changing a student's evaluation.
/// </summary>
public sealed class EvaluationService(
    IStudentRepository studentRepository,
    IRubricRepository rubricRepository,
    IEvaluationRepository evaluationRepository,
    EvaluationFactory evaluationFactory)
{
    public Evaluation SetScore(Guid studentId, Guid criterionId, decimal points, string? comment)
    {
        var student = studentRepository.GetById(studentId)
            ?? throw new InvalidOperationException("Student was not found.");
        var criterion = rubricRepository.GetById(criterionId)
            ?? throw new InvalidOperationException("Criterion is not in the rubric.");
        var currentEvaluation = evaluationRepository.GetByStudentAndCriterion(studentId, criterionId);

        var evaluation = evaluationFactory.Create(
            currentEvaluation?.Id ?? Guid.NewGuid(),
            student,
            criterion,
            points,
            comment);

        evaluationRepository.Save(evaluation);
        return evaluation;
    }

    public IReadOnlyList<Evaluation> ListForStudent(Guid studentId)
    {
        if (studentRepository.GetById(studentId) is null)
            throw new InvalidOperationException("Student was not found.");

        return evaluationRepository.ListByStudent(studentId);
    }
}
