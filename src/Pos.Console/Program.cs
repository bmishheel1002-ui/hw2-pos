using Microsoft.Extensions.DependencyInjection;
using Pos.Application;
using Pos.Application.Ports;
using Pos.Domain;
using Pos.Infrastructure;
using Pos.ConsoleApp;

var services = new ServiceCollection();
services.AddSingleton<IStudentRepository, JsonStudentRepository>();
services.AddSingleton<IRubricRepository, JsonRubricRepository>();
services.AddSingleton<IEvaluationRepository, JsonEvaluationRepository>();
services.AddSingleton<IGradeExporter, CsvGradeExporter>();
services.AddSingleton<IGradeCalculationStrategy, WeightedAverageGradeStrategy>();
services.AddSingleton<EvaluationFactory>();
services.AddSingleton<FinalGradeCalculator>();
services.AddSingleton<StudentService>();
services.AddSingleton<RubricService>();
services.AddSingleton<EvaluationService>();
services.AddSingleton<GradeReportService>();
services.AddSingleton<PosFacade>();
services.AddSingleton<TextReader>(_ => Console.In);
services.AddSingleton<TextWriter>(_ => Console.Out);
services.AddSingleton<PosConsoleApplication>();

using var provider = services.BuildServiceProvider();
provider.GetRequiredService<PosConsoleApplication>().Run();
