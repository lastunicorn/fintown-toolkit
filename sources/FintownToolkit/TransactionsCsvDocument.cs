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
			TransactionsCsvDocument document = new TransactionsCsvDocument();

			using StreamReader reader = File.OpenText(filePath);
			await document.LoadAsync(reader);

			return document;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException("Failed to load transactions CSV file.", ex);
		}
	}

	private async Task LoadAsync(StreamReader reader)
	{
		// The first 3 lines contain metadata and are intentionally ignored.
		SkipLine(reader, 1);
		SkipLine(reader, 2);
		SkipLine(reader, 3);

		int lineNumber = 4;
		string headerLine = await reader.ReadLineAsync() ?? throw new FormatException("CSV file does not contain a column header line.");
		List<string> headers = ParseCsvLine(headerLine, lineNumber);

		if (headers.Count == 0)
			throw new FormatException("CSV header line is empty.");

		columnHeaders.Clear();
		columnHeaders.AddRange(headers);

		while (!reader.EndOfStream)
		{
			lineNumber++;
			string line = await reader.ReadLineAsync();

			if (string.IsNullOrWhiteSpace(line))
				continue;

			List<string> fields = ParseCsvLine(line, lineNumber);

			if (fields.Count != headers.Count)
				throw new FormatException($"CSV line {lineNumber} has {fields.Count} columns, but {headers.Count} were expected.");

			Transactions.Add(ParseTransaction(fields, lineNumber));
		}
	}

	private static void SkipLine(StreamReader reader, int lineNumber)
	{
		if (reader.ReadLine() is null)
			throw new FormatException($"CSV file ended before header line {lineNumber}.");
	}

	private static TransactionRecord ParseTransaction(List<string> fields, int lineNumber)
	{
		DateTime date = DateTime.ParseExact(fields[4], "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
		(decimal amount, string currency) = ParseAmount(fields[2], lineNumber);

		return new TransactionRecord
		{
			Description = fields[0],
			Project = fields[1],
			Amount = amount,
			Currency = currency,
			Status = fields[3],
			Date = date
		};
	}

	private static (decimal Amount, string Currency) ParseAmount(string amountText, int lineNumber)
	{
		string[] parts = amountText.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length != 2)
			throw new FormatException($"Invalid amount format at line {lineNumber}: '{amountText}'.");

		if (!decimal.TryParse(parts[0], System.Globalization.NumberStyles.Number, System.Globalization.CultureInfo.InvariantCulture, out decimal amount))
			throw new FormatException($"Invalid numeric amount at line {lineNumber}: '{parts[0]}'.");

		return (amount, parts[1]);
	}

	private static List<string> ParseCsvLine(string line, int lineNumber)
	{
		List<string> fields = [];
		System.Text.StringBuilder currentField = new();
		bool inQuotes = false;

		for (int i = 0; i < line.Length; i++)
		{
			char currentChar = line[i];

			if (currentChar == '"')
			{
				if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
				{
					currentField.Append('"');
					i++;
				}
				else
				{
					inQuotes = !inQuotes;
				}

				continue;
			}

			if (currentChar == ',' && !inQuotes)
			{
				fields.Add(currentField.ToString());
				currentField.Clear();
				continue;
			}

			currentField.Append(currentChar);
		}

		if (inQuotes)
			throw new FormatException($"Unterminated quoted field at line {lineNumber}.");

		fields.Add(currentField.ToString());

		return fields;
	}
}