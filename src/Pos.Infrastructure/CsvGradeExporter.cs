using System.Globalization;
using System.Text;
using Pos.Application;
using Pos.Application.Ports;

namespace Pos.Infrastructure;

/// <summary>
/// Exports current grade reports as a simple CSV file.
/// </summary>
public sealed class CsvGradeExporter(string filePath = "output/final-grades.csv") : IGradeExporter
{
    public string Export(IReadOnlyList<StudentGradeReport> reports)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
            Directory.CreateDirectory(directory);

        var lines = new List<string> { "StudentId,StudentName,FinalGrade,EvaluatedCriteria,TotalCriteria" };
        lines.AddRange(reports.Select(report => string.Join(
            ',',
            Escape(report.Student.Id.ToString()),
            Escape(report.Student.Name),
            Escape(report.FinalGrade?.Points.ToString("0.00", CultureInfo.InvariantCulture) ?? ""),
            Escape(report.EvaluatedCriteriaCount.ToString(CultureInfo.InvariantCulture)),
            Escape(report.TotalCriteriaCount.ToString(CultureInfo.InvariantCulture)))));

        File.WriteAllText(filePath, string.Join(Environment.NewLine, lines), new UTF8Encoding(false));
        return Path.GetFullPath(filePath);
    }

    private static string Escape(string value)
    {
        return $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\"";
    }
}
