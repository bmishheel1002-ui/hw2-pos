# POS - Homework 2 submission

POS is a C#/.NET 10 console application for recording teacher evaluations of students.
It stores data in JSON files and exports current final grades to CSV.

## Run the application

From the `hw2-pos` project directory, run:

```bash
dotnet run --project src/Pos.Console/Pos.Console.csproj
```

The app creates `data/students.json`, `data/criteria.json`, and
`data/evaluations.json` when you add data. Choose menu item `8` to create
`output/final-grades.csv`.

## Console walkthrough

Use the menu in this order to demonstrate the full scenario:

1. Add a student with option `1`; keep the ID shown by the app.
2. Add two criteria with option `3`; for example, `Quiz` (maximum `10`, weight
   `1`) and `Project` (maximum `20`, weight `3`). Keep both criterion IDs.
3. Set scores with option `5`, using the student ID and each criterion ID.
   Scores outside the selected criterion's range are rejected.
4. Show a final grade with option `7`. The report shows the grade and how many
   criteria have a score.
5. Export all current results with option `8`.

The grade is a weighted average on a ten-point scale:

```text
sum((score / criterion maximum) * criterion weight)
----------------------------------------------------- * 10
             sum(weights for scored criteria)
```

The result uses only criteria that already have a score. The report shows the
number of scored criteria, so a result can be recognized as incomplete.

## Domain model and rules

- `Student` is an Entity: its identity is its ID, and its name is checked.
- `Criterion` is an Entity: it has an ID, a name, a maximum score, and a positive
  weight.
- `Score` is a Value Object: its points must be between zero and the criterion's
  maximum.
- `Evaluation` is an Entity: it connects one student, one rubric criterion, a
  score, a date, and an optional comment.
- `EvaluationFactory` creates an evaluation from a real student and criterion.
  `EvaluationService` first checks that both exist, so an evaluation cannot be
  added for a criterion outside the rubric.
- `FinalGradeCalculator` is a Domain Service. It uses a calculation strategy
  because the final-grade rule is not owned by one student or one criterion.

The ubiquitous language in the code uses the assignment's terms: `Student`,
`Criterion`, `Evaluation`, `Score`, `Rubric`, and `FinalGrade`.

## Architecture and patterns

This project uses a small Ports and Adapters structure. The source
dependencies point toward the domain:

```text
Pos.Console -> Pos.Infrastructure -> Pos.Application -> Pos.Domain
                         \\-------------------------------> Pos.Domain
Pos.Tests -------------------------> Pos.Application / Pos.Domain
```

- `Pos.Domain` contains entities, the `Score` value object, domain rules, the
  factory, and the grade-calculation strategy. It does not reference console,
  JSON, or CSV code.
- `Pos.Application` contains use cases and repository/exporter ports.
- `Pos.Infrastructure` implements the ports with JSON files and CSV export.
- `Pos.Console` is the input/output adapter and composition root. It registers
  the adapters and application services in the DI container.

GoF patterns used:

- **Factory (creational):** `EvaluationFactory` creates a valid evaluation and
  keeps score construction in one place.
- **Facade (structural):** `PosFacade` gives the console one simple entry point
  instead of making the menu coordinate four application services.
- **Strategy (behavioral):** `IGradeCalculationStrategy` and
  `WeightedAverageGradeStrategy` keep the weighted-grade rule replaceable without
  changing the entities or console. A second strategy,
  `WeightedAverageIncludingMissingAsZeroStrategy`, shows the alternative rule for
  criteria that do not have a score yet. The application uses the first policy;
  it can be changed in the composition root in `Program.cs`.

## Tests

Run the tests with:

```bash
dotnet test tests/Pos.Tests/Pos.Tests.csproj
```

The tests cover score and criterion invariants, weighted calculation, rejecting
criteria outside the rubric, updating an existing evaluation, and reports before
any scores exist.

## AI assistance

AI asssistance was used to review the structure, tests, and English comments. 
