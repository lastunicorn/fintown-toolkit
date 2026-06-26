using System.Globalization;
using CsvHelper;
using DustInTheWind.Machina;

namespace DustInTheWind.Fintown.Toolkit.Csv.States;

internal class ReadDocumentHeaderState : IState<CsvReadState, CsvReadContext>
{
	public CsvReadState Id => CsvReadState.DocumentHeader;

	public async Task<CsvReadState?> ExecuteAsync(CsvReadContext context)
	{
		await Parse(context);

		// CsvDocumentHeader header = await CsvDocumentHeader.Parse(context.CsvReader);
		// context.Document.DateFrom = header.DateFrom;
		// context.Document.DateTo = header.DateTo;

		return CsvReadState.ColumnsHeader;
	}

	private static async Task Parse(CsvReadContext context)
	{
		CsvReader csvReader = context.CsvReader;

		if (!await csvReader.ReadAsync())
			throw new DocumentLoadException("CSV file ended before line 1 (Transaction history).");

		string[] titleRecord = csvReader.Parser.Record ?? [];
		if (titleRecord.Length == 0 || !string.Equals(titleRecord[0], "Transaction history", StringComparison.OrdinalIgnoreCase))
			throw new DocumentLoadException("CSV line 1 must contain 'Transaction history'.");

		if (!await csvReader.ReadAsync())
			throw new DocumentLoadException("CSV file ended before line 2 (From date).");
		DateOnly dateFrom = ParseHeaderDate(csvReader.Parser.Record ?? [], "From", 2);

		if (!await csvReader.ReadAsync())
			throw new DocumentLoadException("CSV file ended before line 3 (To date).");
		DateOnly dateTo = ParseHeaderDate(csvReader.Parser.Record ?? [], "To", 3);

		context.Document.DateFrom = dateFrom;
		context.Document.DateTo = dateTo;
	}

	private static DateOnly ParseHeaderDate(IReadOnlyList<string> record, string expectedLabel, int lineNumber)
	{
		if (record.Count < 2)
			throw new DocumentLoadException($"CSV line {lineNumber} must contain '{expectedLabel}' and a date.");

		if (!string.Equals(record[0], expectedLabel, StringComparison.OrdinalIgnoreCase))
			throw new DocumentLoadException($"CSV line {lineNumber} must start with '{expectedLabel}'.");

		if (!DateOnly.TryParseExact(record[1], "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly date))
			throw new DocumentLoadException($"CSV line {lineNumber} contains an invalid date '{record[1]}'. Expected format: yyyy-MM-dd.");

		return date;
	}
}