# DemoXunitv3SolutionAnalyzerCodeFix

Local .NET 10 xUnit analyzer/code-fix smoke test repository for the missing cancellation token Roslyn analyzer and code fix.

## Purpose
- Build and run a realistic test harness locally.
- Keep verification deterministic and close to a remote package/CI-style workflow.
- Demonstrate analyzer + code-fix behavior with xUnit v3.

## Build and run

```bash
dotnet build
```

The stable verification command for this repo is to run the generated xUnit executable directly (this avoids adapter variance with .NET 10 preview environments):

```bash
dotnet DemoXunitv3SolutionAnalyzerCodeFix/bin/Debug/net10.0/DemoXunitv3SolutionAnalyzerCodeFix.dll
```

