# Fintown Toolkit

[![GitHub Repo](https://img.shields.io/badge/github-repo-blue?logo=github)](https://github.com/lastunicorn/fintown-toolkit) [![GitHub Build](https://img.shields.io/github/actions/workflow/status/lastunicorn/fintown-toolkit/build-master.yml?logo=github)](https://github.com/lastunicorn/fintown-toolkit/actions/workflows/build-master.yml) [![NuGet Version](https://img.shields.io/nuget/v/DustInTheWind.Fintown.Toolkit?logo=nuget)](https://www.nuget.org/packages/DustInTheWind.Fintown.Toolkit) [![NuGet Downloads](https://img.shields.io/nuget/dt/DustInTheWind.Fintown.Toolkit?logo=nuget)](https://www.nuget.org/packages/DustInTheWind.Fintown.Toolkit)

`Fintown Toolkit` is a .NET library for working with files exported from Fintown.

Fintown is an loan investment platform powered by Vihorev Group based in Czech Republic.

- https://fintown.eu

The package is published as `DustInTheWind.Fintown.Toolkit`.

## Installation

Package Manager:

```powershell
Install-Package DustInTheWind.Fintown.Toolkit
```

.NET CLI:

```bash
dotnet add package DustInTheWind.Fintown.Toolkit
```

## Runtime Requirements

- Library target framework: `.NET 8.0` (`net8.0`)

## Quick Start

### a) Export the Transactions CSV File

In Fintown web application:

1. Log in.
2. Open **My Account**.
3. Open **Transactions**.
4. Select the date interval you need.
5. Click the **CSV** button to download the file.

You will get a CSV containing transaction rows that can be parsed with this toolkit.

### b) Parse the Exported Document

Create a small console app and parse your exported file:

```csharp
using DustInTheWind.Fintown.Toolkit;

TransactionsDocument transactionsDocument = await TransactionsDocument.LoadFromFileAsync("transactions.csv");

foreach (TransactionRecord transaction in transactionsDocument)
{
	...
}
```

## `Transaction` Record

Each row is mapped to:

- `Description` (`TransactionDescription`)
  
- `Project` (`string`)
- `Amount` (`CurrencyAmount`)
- `Status` (`TransactionStatus`)
- `Date` (`DateTime`)

## Demo Project

The repository includes a sample CLI project in `sources/Fintown.Toolkit.Demo` that demonstrates:

- reading `transactions.csv`
- printing parsed data.

You can use this project as a reference implementation for your own importer/exporter tools.
