# CliUtils — AI Context

## What this project is

Shared .NET 10 class library for enhanced console output (colored output,
logging, Spectre.Console wrappers). No standalone entry point — consumed via
`ProjectReference` by other projects in this repository.

| File | Purpose |
| :--- | :------ |
| `ConsoleColorHelper.cs` | Static helper for colored output (success/error/warning/info/debug) |
| `ConsoleHelper.cs` | Backward-compatible wrapper around Spectre.Console `AnsiConsole` |
| `ConsoleLogger.cs` | In-memory logger with timestamps + categories |
| `RichConsole.cs` | Rich text rendering via Spectre.Console |

## Commands

- **Build**: `dotnet build CliUtils/CliUtils.csproj` (from repo root)
- **Test**: no test project

## Conventions

- .NET 10.0, `Directory.Build.props` applies repo-wide settings.
- Dependency: Spectre.Console 0.57.2.
- Consumers: `CsvTranslations/` apps (OllamaTranslatorApp, TextToSpeechApp, etc.)
- Builds identically on Windows and Linux (`dotnet build CliUtils/CliUtils.csproj`);
  it is also built as a project of `CsvTranslations.sln`.

## Gotchas

- Library only — do not add a `Main`; no `dotnet run` entry point.
- If a new CLI app needs console output, extend these utilities rather than
  creating new overlapping loggers (see root Copilot instructions on DRY).
- It targets `net10.0`, and the .NET 10 runtime is installed, so consumers run
  directly — no roll-forward needed.

## Related docs

- `README.md` — feature list and usage notes