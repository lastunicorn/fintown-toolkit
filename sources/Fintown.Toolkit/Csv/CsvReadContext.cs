using CsvHelper;

namespace DustInTheWind.Fintown.Toolkit.Csv;

internal class CsvReadContext
{
	public CsvReader CsvReader { get; init; }

	public TransactionsDocument Document { get; init; }

	public string[] ColumnHeaders { get; set; }
}