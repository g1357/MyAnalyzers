# MyAnalyzers

Roslyn analyzers (for Visual Studio 2026 / .NET) enforcing **C# naming conventions**.

## Install
Reference the NuGet package from your consuming project:

```bash
dotnet add package MyAnalyzers.Naming
```

> Package id may differ slightly depending on the final project name in this repository; see the `.csproj` `PackageId`.

## Rules
The package contains analyzers (and code fixes where applicable) for:

- **MYAN0001** — Interfaces must be `PascalCase` and start with `I` (e.g. `IUserService`).
- **MYAN0002** — Types (`class`, `struct`, `enum`, `delegate`) must be `PascalCase`.
- **MYAN0003** — Members (`method`, `property`, `event`) must be `PascalCase`.
- **MYAN0004** — Parameters must be `camelCase`.
- **MYAN0005** — Local variables must be `camelCase`.
- **MYAN0006** — Private fields must be `_camelCase`.
- **MYAN0007** — Async methods returning `Task`/`Task<T>`/`ValueTask`/`ValueTask<T>` must end with `Async` (event handlers are excluded by default).

## Configuration (.editorconfig)
All rules can be configured via standard analyzer severity settings:

```ini
# example
[*.cs]
dotnet_diagnostic.MYAN0006.severity = warning
# dotnet_diagnostic.MYAN0006.severity = error
# dotnet_diagnostic.MYAN0006.severity = none
```

## Build

```bash
dotnet test
dotnet pack
```
