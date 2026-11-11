# CliUtils — Working Instructions

| | |
| :--- | :--- |
| **Need** | A session that lands here cold: what this library is, how to build it, where the agent configuration now lives, and which documented claims are no longer true of the code. |
| **Why** | The repository is a published NuGet package with a live commit gate. A wrong assumption either blocks a commit or ships a package whose consumers break. |
| **What** | The single project, the build and pack commands, the `.agents/` configuration, the real public API, the commit gate's behaviour, and the doc drift already found. Deliberately not repeated: `PUBLISHING.md`, which owns the release procedure. |
| **DoD** | A session can build, change the public surface safely, commit through the gate, and explain any file under `.agents/` without reading the git history. |
| **Top risk** | Changing the public API without a version bump, so the consumer in the prototype-k umbrella keeps restoring the old build from nuget.org and reports the change as broken. |

Loaded automatically as workspace instructions for anything under `cliutils/`.

## What this repository is

One .NET 10 class library, `CliUtils.csproj`, with no application, no solution and no test project. It provides coloured console output, Spectre.Console wrappers and an in-memory log list. It ships as the NuGet package **`DynamicDisorder.CliUtils`** — the prefix is mandatory, because an unrelated `CliUtils` package owns the bare id on nuget.org.

The only consumer is the **prototype-k** umbrella (`prototype-k/CsvTranslations/`), which pins the version through Central Package Management and restores it from nuget.org. Nothing inside this repository references the library.

`README.md` describes the four features. `PUBLISHING.md` owns everything about releases: package identity, trusted publishing, the reproducible-build properties, and the consumer bump that must accompany a release. Read it before touching `<Version>` or any build property.

## Layout

| Path | Holds |
| :--- | :--- |
| `ConsoleColorHelper.cs`, `ConsoleHelper.cs`, `ConsoleLogger.cs`, `RichConsole.cs` | The entire public surface |
| `.agents/skills/` | Seven tracked, tool-neutral skills (build-verify, conventions, refactor, explain, commit, investigate, spike) |
| `.agents/hooks/pre-commit-build-check.sh` | The build gate — one file, one entry point |
| `.github/workflows/publish.yml` | Tag-triggered Trusted Publishing |
| `local_dev/scripts/sandbox-env.sh` | Redirects tool caches for a read-only `$HOME` |
| `local_user_files/` | Git-ignored scratch; investigations and spikes are written here |

## Build and pack

Measured on 2026-11-10: `dotnet --version` reports `10.0.301`, and the project targets `net10.0`.

```bash
dotnet build CliUtils.csproj --no-incremental
dotnet pack -c Release
```

`--no-incremental` matters: it stops a stale `obj/` from hiding an error. `dotnet build` restores on its own, so never run `dotnet restore` first.

Where `$HOME` is not writable — the agent sandbox on this machine — the first restore fails with `Read-only file system : '~/.nuget/packages'`. Source the redirect script first, in a shell that can `source`:

```bash
source local_dev/scripts/sandbox-env.sh
dotnet build CliUtils.csproj --no-incremental
```

It exports `NUGET_PACKAGES`, `DOTNET_CLI_HOME` and the `XDG_*` paths into `local_user_files/`, derived from the script's own location, so it hard-codes no machine path. Harmless on a normal machine.

The project carries no `Directory.Build.props` and no `Directory.Packages.props`; every build setting lives in `CliUtils.csproj`.

## The commit gate

`git config core.hooksPath` is `.githooks`, so every commit runs `.githooks/pre-commit`, a two-line wrapper that execs `.agents/hooks/pre-commit-build-check.sh`. The git hook is the only entry point; there is no harness hook to rely on.

Behaviour worth knowing before committing:

- It exits 0 immediately unless something staged matches `.cs`, `.csproj`, `.props` or `.targets` — a docs-only commit costs nothing.
- With no `.sln` in the repository it falls back to the project and builds that, so it does gate here. That was a defect once: a single-project repository used to pass silently.
- Exit 2 blocks the commit and returns stderr as the reason. It retries a failed build once, because `--no-incremental` can lose a race with the Windows file lock.
- A read-only workspace makes it warn and allow, rather than block on `MSB3061`.
- In harness mode it checks `.cwd` against `CLAUDE_PROJECT_DIR`, so the bridge's process-wide mount does not build this repository from another workspace.

The git hook above is the gate that always runs; there is no settings file to arm.

Both scripts must keep their executable bit (`100755` in the git index). They were stored `100644`, which is invisible on Windows — where MSYS marks every file executable — and fatal on Linux: git skips a non-executable hook without a message, and the wrapper's `[ -x "$gate" ]` test would exit 0. Check with `git ls-files -s .githooks .agents/hooks` after any move.

## Public API

| Type | What it offers |
| :--- | :--- |
| `ConsoleColorHelper` | Static coloured output through `Console.ForegroundColor`: `WriteSuccess`/`WriteError`/`WriteWarning`/`WriteInfo`/`WriteDebug`, `WriteTranslation`, `WriteFileResult` |
| `ConsoleHelper` | The same member names reimplemented on Spectre.Console `AnsiConsole`, with markup escaping |
| `ConsoleLogger` | A static `List<ConsoleLogEntry>` and `WriteLine`, which appends to it |
| `RichConsole` | `WriteTable<T>`, `WithProgressAsync`, `WithLiveProgressAsync`, `Ask<T>`, `Select`, `MultiSelect<T>`, `Confirm`, `WithStatusAsync`, `WritePanel`, `CreateLiveDisplay`, `WriteTree`, `WriteTranslation` |

`ConsoleHelper` and `ConsoleColorHelper` are deliberately overlapping: the former is the Spectre path, the latter the raw-console path kept for consumers that predate it. Add new output helpers to one of them; a third parallel set is the DRY failure this library already carries.

Four behaviours are not obvious from the signatures:

- **Escaping is partial.** `WriteSuccess`, `WriteError`, `WriteWarning`, `WriteInfo`, `WriteDebug` and `WriteTranslation` escape `[` and `]`; `WriteLine(string, ConsoleColor)` does not, and `AnsiConsole.WriteLine` parses markup, so a bracketed message on that path is interpreted rather than printed.
- **`RichConsole.WriteTranslation` passes its `[yellow]`/`[green]` tags to `new Text(...)`.** `Text` does not parse markup; only `Markup` does. Expect the tags to appear literally. Not yet confirmed on a real terminal.
- **`WriteTable<T>` reads properties by name reflection** (`typeof(T).GetProperty`). A name that does not match yields an empty cell rather than an error, so a renamed property fails silently.
- **`Ask<T>` returns `T?`** and shows the default; do not assume a non-null result.

`ConsoleLogger` is misnamed in practice: it writes nothing to the console, and `ConsoleLogEntry` holds only the text.

## Conventions

The C# rules that analyzers cannot enforce live in the `csharp-conventions` skill — no expression-bodied members, `is null` over `== null`, XML docs on public and internal members, and C# keywords rather than BCL type names. `.editorconfig` remains the source of truth for everything it does cover; read it instead of restating it.

Two rules that keep the tree committable:

- **LF everywhere**, repository and working tree (`.gitattributes`). Never introduce CRLF.
- **Never hard-code a machine path** — build paths with `Path.Combine`. A commit message is public text: no account, profile or host name, and no description of content that was removed.

Commit subjects are Conventional Commits, imperative, under 50 characters. The `git-commit` skill has the full rules; the release tags are `vX.Y.Z` and drive the publish workflow.

## Verification

```bash
dotnet build CliUtils.csproj --no-incremental   # exit 0; report every error with file and line
bash -n .agents/hooks/pre-commit-build-check.sh # syntax only, no execution
```

To confirm a refactor did not leak into the tree, also run `git status --short` and check that no `bin/`, `obj/` or `local_user_files/` path is staged.

When a build property changes, re-run the path-redaction check in `PUBLISHING.md` before packing. Losing `PathMap` is silent, and the leak would ship inside both the package and the symbols package.

## Known drift — do not rediscover these

| Claim or symptom | Reality | Action |
| :--- | :--- | :--- |
| `ConsoleLogger` "captures log entries with timestamps and categories" (README, and this file's predecessor) | `ConsoleLogEntry` holds one string and nothing else | Correct the description where it appears; do not add fields without a version bump |
| Consumers are the `CsvTranslations/` apps "in this repository", built by `CsvTranslations.sln` | No solution and no `CsvTranslations` here; the consumer is the prototype-k umbrella, restored from nuget.org | Treat prototype-k as the consumer |
| `sandbox-env.sh` usage shows `./local_dev/scripts/build-all-linux.sh --test` | That script does not exist in this repository | Ignore the example; run `dotnet build` directly |
| `bin/` and `obj/` exist in the working tree | Git-ignored build output | Never stage, never delete as cleanup |

## Where work is recorded

Investigations (`issue-investigate`) and research (`spike`) write dated Markdown into `local_user_files/`, which is git-ignored except for its README. Nothing durable belongs there — promote anything worth keeping into the repository proper. A finding that only the owner can confirm stays an open item in the reply, not a claim in a file.

Documentation: this file is the only instruction surface. The per-tool files (`.clinerules`, `.claudeignore`, `.github/copilot-instructions.md`, `.agents/settings.json`) were removed on 2026-11-11 so nothing duplicates it.
