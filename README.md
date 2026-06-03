# Fintown Toolkit

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

TransactionsDocument document = TransactionsDocument.LoadFromFile("transactions.csv");

foreach (TransactionRecord transaction in document)
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
