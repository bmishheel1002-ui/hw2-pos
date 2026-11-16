namespace Pos.Application.Ports;

using Pos.Application;

/// <summary>
/// An outgoing port for exporting final-grade reports.
/// </summary>
public interface IGradeExporter
{
    string Export(IReadOnlyList<StudentGradeReport> reports);
}
