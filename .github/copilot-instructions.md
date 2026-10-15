# GitHub Copilot - Assistant Instructions for this repository

## Purpose
- Workspace-wide behaviour for Copilot suggestions here.

## Guidelines
- Be concise and direct; no filler, no apologies.
- Keep diffs minimal and focused; follow the repository's existing style.
- When unsure, ask rather than guess.
- No secrets, credentials or external network calls in examples.

## Repository facts
- Build: `dotnet build CliUtils.csproj`; pack: `dotnet pack -c Release`.
- A single library project: keep the public surface documented and the package metadata
  (`PackageId`, `Version`, license, repository URL) in step with releases.
- `.editorconfig` is the source of truth for formatting, and line endings are LF everywhere.
- Never hard-code a machine path. Never name a user folder, profile path or account in a commit,
  and never describe content that was removed or anonymised.
- Local scratch lives in `local_user_files/` (git-ignored).
