using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace DustInTheWind.FintownToolkit;

public partial class TransactionsCsvDocument
{
	public DateOnly DateFrom { get; set; }

	public DateOnly DateTo { get; set; }

	private readonly List<string> columnHeaders = [];

	public List<string> ColumnHeaders => columnHeaders;

	public List<TransactionRecord> Transactions { get; } = [];

	public static async Task<TransactionsCsvDocument> LoadFromFile(string filePath)
	{
		if (filePath == null) throw new ArgumentNullException(nameof(filePath));

		if (string.IsNullOrWhiteSpace(filePath))
			throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

		try
		{
			TransactionsCsvDocument document = new();

			using StreamReader streamReader = File.OpenText(filePath);
			await document.LoadAsync(streamReader);

			return document;
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException("Failed to load transactions CSV file.", ex);
		}
	}

	private async Task LoadAsync(StreamReader streamReader)
	{
		CsvConfiguration configuration = new(CultureInfo.InvariantCulture)
		{
			HasHeaderRecord = false,
			IgnoreBlankLines = true
		};

		using CsvReader csvReader = new(streamReader, configuration);

		for (int i = 1; i <= 3; i++)
		{
			if (!await csvReader.ReadAsync())
				throw new DocumentLoadException($"CSV file ended before header line {i}.");
		}

		if (!await csvReader.ReadAsync())
			throw new DocumentLoadException("CSV file does not contain a column header line.");

		string[] headers = csvReader.Parser.Record ?? [];

		if (headers.Length == 0)
			throw new DocumentLoadException("CSV header line is empty.");

		columnHeaders.Clear();
		columnHeaders.AddRange(headers);

		while (await csvReader.ReadAsync())
		{
			string[] fields = csvReader.Parser.Record ?? [];
			int lineNumber = csvReader.Parser.RawRow;

			if (fields.Length != headers.Length)
				throw new FormatException($"CSV line {lineNumber} has {fields.Length} columns, but {headers.Length} were expected.");

			Transactions.Add(ParseTransaction(fields, lineNumber));
		}
	}

	private static TransactionRecord ParseTransaction(IReadOnlyList<string> fields, int lineNumber)
	{
		try
		{
			DateTime date = DateTime.ParseExact(fields[4], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
			CurrencyAmount amount = CurrencyAmount.Parse(fields[2]);

			return new TransactionRecord
			{
				Description = fields[0],
				Project = fields[1],
				Amount = amount,
				Status = fields[3],
				Date = date
			};
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException($"Invalid line {lineNumber}", ex);
		}
	}
}