# CliUtils

A shared .NET 10 class library providing enhanced console output utilities for CLI applications across the repository.

## Features

- **ConsoleColorHelper** — Static helper for colored console output (success/green, error/red, warning/yellow, info/cyan, debug/gray). Includes dedicated methods for displaying translation pairs and file operation results with color-coding.
- **ConsoleHelper** — Backward-compatible wrapper around Spectre.Console's `AnsiConsole`, providing the same color methods with markup escaping.
- **ConsoleLogger** — Simple in-memory logger that captures log entries with timestamps and categories.
- **RichConsole** — Advanced console rendering using Spectre.Console for rich text formatting.

## Dependencies

- .NET 10.0
- Spectre.Console 0.57.2

## Usage

This library is referenced as a `ProjectReference` by other projects in the repository. Build the solution or project that depends on it — there is no standalone entry point.

```bash
dotnet build CliUtils/CliUtils.csproj