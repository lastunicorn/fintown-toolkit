# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## About

`Fintown Toolkit` is a .NET 8.0 library (`DustInTheWind.Fintown.Toolkit` on NuGet) for parsing CSV transaction exports from the Fintown loan investment platform.

## Build & Run

```bash
# Restore and build (solution file at repo root)
dotnet restore ./Fintown.Toolkit.slnx --configfile ./nuget.config
dotnet build ./Fintown.Toolkit.slnx
dotnet build ./Fintown.Toolkit.slnx -c Release

# Run the demo (requires a transactions.csv in the working dir)
dotnet run --project sources/Fintown.Toolkit.Demo
```

There are currently no test projects in the solution.

## Architecture

### CSV format

Fintown exports have a 4-line preamble before data rows:
1. `"Transaction history"`
2. `"From","<yyyy-MM-dd>"`
3. `"To","<yyyy-MM-dd>"`
4. Column header row (5 columns: Description, Project, Amount, Status, Date)

`CsvDocumentReadState` is a state machine enum that enforces this read order inside `CsvTransactionsDocument`.

### Public API surface

`TransactionsDocument` is the single entry point. It exposes six `LoadAsync` overloads accepting a file path, raw CSV string, `Stream`, `FileInfo`, `StreamReader`, or `TextReader`. All delegate to the internal `CsvTransactionsDocument` reader.

Each row maps to `TransactionRecord` (a `record class`) with five properties:

| Property | Type |
|---|---|
| `Description` | `TransactionDescription` |
| `Project` | `string` |
| `Amount` | `CurrencyAmount` |
| `Status` | `TransactionStatus` |
| `Date` | `DateTime` |

### Value types

`TransactionDescription`, `CurrencyAmount`, and `TransactionStatus` all carry implicit conversions to/from `string`. `TransactionDescription` and `TransactionStatus` expose well-known static instances (e.g. `TransactionDescription.InvestingFunds`). `TransactionDescription` also has a `KnownValues` collection.

### Internal CSV layer

`sources/Fintown.Toolkit/Csv/` contains the internal CSV parsing (using [CsvHelper](https://joshclose.github.io/CsvHelper/)) and is not exposed in the public API.

### XML documentation

Only add XML `<summary>` docs to public types/members — they are packaged with the NuGet artifact. Internal types should not have XML docs.

## Code Conventions

- **No `var`** — always use the explicit type.
- **`new()` syntax** for object instantiation.
- **LINQ lambda parameter** named `x`.
- **Object initializers** with more than one property: each property on its own line.
- **No braces** for single-line `if`, `for`, or `using` bodies.
- **Test naming**: `Having<SetupDetails>_When<Action>_Then<ExpectedResult>`.
- **Test file layout**: one file per method under a `<ClassName>Tests/` directory (e.g. `ColorTests/QueryTests.cs`).
- **`Assert.Throws`**: always use a block-body lambda `() => { ... }`.
