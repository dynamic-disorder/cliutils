# Publishing DynamicDisorder.CliUtils

How this package reaches nuget.org, and what has to move in step with it.

## Package identity

| | |
| :--- | :--- |
| **Package ID** | `DynamicDisorder.CliUtils` |
| **Version** | the `<Version>` element in `CliUtils.csproj` — the source of truth |
| **License** | MIT, declared as `PackageLicenseExpression` and shipped as `LICENSE` |
| **Repository** | <https://github.com/dynamic-disorder/cliutils> |
| **Symbols** | `IncludeSymbols` with `SymbolPackageFormat=snupkg` |
| **Readme** | `README.md`, packed into the package through `PackageReadmeFile` |
| **nuget.org page** | <https://www.nuget.org/packages/DynamicDisorder.CliUtils> |

The ID is prefixed because the unprefixed name is already taken on nuget.org. Never publish or
reference an unprefixed `CliUtils` package — a different package carries that name, and it would be
resolved instead of this one.

## One-time setup

1. A nuget.org account.
2. An API key from <https://www.nuget.org/account/apikeys>:
   - **Scope**: *Push new packages and package versions*
   - **Glob pattern**: `DynamicDisorder.CliUtils` (or `*`)
3. Keep the key out of this repository: pass it on the command line or read it from a secret store.
   Never commit it, and never put it in a `nuget.config`.

## Release

```bash
# On a machine where $HOME is read-only - the agent sandbox on this box - redirect the package
# caches first by sourcing sandbox-env.sh from the umbrella repository's local_dev/scripts/.
source .../local_dev/scripts/sandbox-env.sh

dotnet pack -c Release
```

That writes two artifacts:

```text
bin/Release/DynamicDisorder.CliUtils.<version>.nupkg     the package
bin/Release/DynamicDisorder.CliUtils.<version>.snupkg    symbols, attached to it by nuget.org
```

Push both. The symbols package is what lets a consumer step into this library while debugging:

```bash
dotnet nuget push bin/Release/DynamicDisorder.CliUtils.<version>.nupkg \
  --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json
dotnet nuget push bin/Release/DynamicDisorder.CliUtils.<version>.snupkg \
  --api-key "$NUGET_API_KEY" --source https://api.nuget.org/v3/index.json
```

**Re-pack before every push.** A `.nupkg` left in `bin/` carries the metadata it was built with: an
artifact from an earlier build here still declared the previous license, and pushing it would have
published the wrong terms.

## Verify

```bash
# 404 means the ID is not published at all; otherwise a JSON list of the versions served.
curl -s https://api.nuget.org/v3-flatcontainer/dynamicdisorder.cliutils/index.json
```

Expect `{"versions":["0.1.0"]}` after the first successful push. Indexing takes a minute or two, so
check the package page before concluding that a push failed.

## Versioning, and the one consumer

Exactly one repository consumes this package: **csv-translations**, through Central Package
Management. A release therefore needs a matching change there:

```xml
<!-- csv-translations/Directory.Packages.props -->
<PackageVersion Include="DynamicDisorder.CliUtils" Version="0.1.0" />
```

Bump that in the same breath as the release, or the consumer keeps restoring the previous version.

Because the consumer resolves this package from nuget.org, a fresh clone of it cannot restore until
the version it pins exists there. That is why the consumer repository is created and pushed only
after the first publish.

## Things that look like shortcuts but are not

- **A local feed in the consumer** — a `nuget.config` pointing at a folder of `.nupkg` files makes a
  clone build early and then rots, hiding the real dependency on nuget.org.
- **Committing the artifacts** — `*.nupkg` and `*.snupkg` are git-ignored on purpose, as are `bin/`
  and `obj/`.
- **Bumping the version without pushing** — consumers resolve what nuget.org serves, not what this
  working tree has built.
