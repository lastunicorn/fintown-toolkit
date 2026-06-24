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

### Projects

| Project | Role |
|---|---|
| `Fintown.Toolkit.Infrastructure` | General-purpose infrastructure — `StateMachine<TState, TContext>` and `IState<TState, TContext>` |
| `Fintown.Toolkit` | Domain library — CSV parsing, public API, NuGet package |
| `Fintown.Toolkit.Demo` | Console app demonstrating the library |

`Fintown.Toolkit` references `Fintown.Toolkit.Infrastructure`.

### State machine (`Fintown.Toolkit.Infrastructure`)

`IState<TState, TContext>` is the interface each state implements. `TState` must be a `struct` enum. `ExecuteAsync` receives the shared context and returns the ID of the next state, or `null` to stop.

`StateMachine<TState, TContext>` dispatches between registered states. Key API:

- Constructor overload accepts a collection of states — the first state registered also becomes `InitialState` if it is not set explicitly.
- `AddState` / `AddState(IEnumerable<...>)` for incremental registration; both are fluent.
- `Start(context)` — sets the context and primes `CurrentState` to `InitialState`.
- `MoveNextAsync()` — executes the current state and advances `CurrentState`. Returns `false` when already stopped.
- `ExecuteAllAsync(context)` — convenience wrapper: calls `Start` then loops `MoveNextAsync` until done.

### CSV format

Fintown exports have a preamble before data rows:
1. `"Transaction history"`
2. `"From","<yyyy-MM-dd>"`
3. `"To","<yyyy-MM-dd>"`
4. Column header row (5 columns: Description, Project, Amount, Status, Date)

### CSV parsing (`Fintown.Toolkit`)

`TransactionsDocument.LoadInternalAsync` is the integration point. It creates the `CsvReader` and `CsvReadContext` directly, wires the state machine, and runs it:

```
CsvReadState enum          CsvReadContext
  DocumentHeader   ──▶   ReadDocumentHeaderState
  ColumnsHeader    ──▶   ReadColumnHeaderState
  DataRows         ──▶   ReadDataRowsState  ──▶  null (done)
```

State classes live in `Csv/States/`. `CsvReadContext` carries the `CsvReader`, the `TransactionsDocument` being built, and the parsed `ColumnHeaders` array.

### Public API surface

`TransactionsDocument` is the single entry point — it extends `Collection<TransactionRecord>` and exposes six `LoadAsync` overloads accepting a file path, raw CSV string, `Stream`, `FileInfo`, `StreamReader`, or `TextReader`.

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
