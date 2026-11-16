using System.Globalization;
using Pos.Application;
using Pos.Domain;

namespace Pos.ConsoleApp;

public sealed class PosConsoleApplication(PosFacade pos, TextReader input, TextWriter output)
{
    public void Run()
    {
        output.WriteLine("POS - Teacher Evaluation of Students");
        output.WriteLine("Data is saved in JSON files in the data folder.");

        while (true)
        {
            PrintMenu();
            var choice = input.ReadLine();

            if (choice is null || choice.Trim() == "0")
                return;

            try
            {
                switch (choice.Trim())
                {
                    case "1": AddStudent(); break;
                    case "2": ListStudents(); break;
                    case "3": AddCriterion(); break;
                    case "4": ListCriteria(); break;
                    case "5": SetScore(); break;
                    case "6": ListEvaluations(); break;
                    case "7": ShowFinalGrade(); break;
                    case "8": ExportFinalGrades(); break;
                    default: output.WriteLine("Choose a number from 0 to 8."); break;
                }
            }
            catch (Exception exception) when (exception is ArgumentException or InvalidOperationException or IOException)
            {
                output.WriteLine($"Cannot complete this action: {exception.Message}");
            }

            output.WriteLine();
        }
    }

    private void PrintMenu()
    {
        output.WriteLine();
        output.WriteLine("1 Add student");
        output.WriteLine("2 List students");
        output.WriteLine("3 Add rubric criterion");
        output.WriteLine("4 List rubric criteria");
        output.WriteLine("5 Set or update a score");
        output.WriteLine("6 List a student's evaluations");
        output.WriteLine("7 Calculate a student's final grade");
        output.WriteLine("8 Export all final grades to CSV");
        output.WriteLine("0 Exit");
        output.Write("Choice: ");
    }

    private void AddStudent()
    {
        output.Write("Student name: ");
        var student = pos.AddStudent(ReadRequiredText());
        output.WriteLine($"Student added. ID: {student.Id}");
    }

    private void ListStudents()
    {
        var students = pos.ListStudents();
        if (students.Count == 0)
        {
            output.WriteLine("No students yet.");
            return;
        }

        foreach (var student in students)
            output.WriteLine($"{student.Id} | {student.Name}");
    }

    private void AddCriterion()
    {
        output.Write("Criterion name: ");
        var name = ReadRequiredText();
        var maximumScore = ReadDecimal("Maximum score: ");
        var weight = ReadDecimal("Weight (relative importance): ");
        var criterion = pos.AddCriterion(name, maximumScore, weight);
        output.WriteLine($"Criterion added. ID: {criterion.Id}");
    }

    private void ListCriteria()
    {
        var criteria = pos.ListCriteria();
        if (criteria.Count == 0)
        {
            output.WriteLine("The rubric is empty.");
            return;
        }

        foreach (var criterion in criteria)
        {
            output.WriteLine(
                $"{criterion.Id} | {criterion.Name} | max {criterion.MaximumScore} | weight {criterion.Weight}");
        }
    }

    private void SetScore()
    {
        var studentId = ReadGuid("Student ID: ");
        var criterionId = ReadGuid("Criterion ID: ");
        var points = ReadDecimal("Score: ");
        output.Write("Comment (optional): ");
        var comment = input.ReadLine() ?? string.Empty;

        var evaluation = pos.SetScore(studentId, criterionId, points, comment);
        output.WriteLine($"Score saved: {evaluation.Score.Points}/{evaluation.Score.MaximumPoints}.");
    }

    private void ListEvaluations()
    {
        var studentId = ReadGuid("Student ID: ");
        var evaluations = pos.ListEvaluations(studentId);
        var criteria = pos.ListCriteria().ToDictionary(criterion => criterion.Id);

        if (evaluations.Count == 0)
        {
            output.WriteLine("This student has no evaluations yet.");
            return;
        }

        foreach (var evaluation in evaluations)
        {
            var criterionName = criteria.TryGetValue(evaluation.CriterionId, out var criterion)
                ? criterion.Name
                : "Unknown criterion";
            output.WriteLine($"{criterionName}: {evaluation.Score.Points}/{evaluation.Score.MaximumPoints}");
            if (!string.IsNullOrWhiteSpace(evaluation.Comment))
                output.WriteLine($"  Comment: {evaluation.Comment}");
        }
    }

    private void ShowFinalGrade()
    {
        var studentId = ReadGuid("Student ID: ");
        var report = pos.GetReport(studentId);

        if (report.FinalGrade is null)
        {
            output.WriteLine($"{report.Student.Name} has no scores yet.");
            return;
        }

        output.WriteLine(
            $"{report.Student.Name}: {report.FinalGrade.Points:0.00}/10 " +
            $"({report.EvaluatedCriteriaCount} of {report.TotalCriteriaCount} criteria evaluated)");
    }

    private void ExportFinalGrades()
    {
        var path = pos.ExportFinalGrades();
        output.WriteLine($"Final grades exported to: {path}");
    }

    private string ReadRequiredText()
    {
        var value = input.ReadLine();
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("This value cannot be empty.");

        return value.Trim();
    }

    private Guid ReadGuid(string prompt)
    {
        output.Write(prompt);
        var value = input.ReadLine();
        if (!Guid.TryParse(value, out var id))
            throw new ArgumentException("Enter a valid ID from the list.");

        return id;
    }

    private decimal ReadDecimal(string prompt)
    {
        output.Write(prompt);
        var value = input.ReadLine();
        var parsed = decimal.TryParse(value, NumberStyles.Number, CultureInfo.CurrentCulture, out var number)
            || decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out number);

        if (!parsed)
            throw new ArgumentException("Enter a valid number.");

        return number;
    }
}
