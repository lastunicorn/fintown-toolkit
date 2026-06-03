using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DustInTheWind.FintownToolkit.Csv;

internal sealed class TransactionsCsvDocument : IDisposable
{
	private readonly CsvReader csvReader;
	private CsvDocumentReadState state;
	private string[] columnHeaders;

	public TransactionsCsvDocument(StreamReader streamReader)
	{
		if (streamReader == null) throw new ArgumentNullException(nameof(streamReader));

		CsvConfiguration configuration = new(CultureInfo.InvariantCulture)
		{
			HasHeaderRecord = false,
			IgnoreBlankLines = true
		};

		csvReader = new(streamReader, configuration);
	}

	public async Task<TransactionsCsvHeader> ReadDocumentHeader()
	{
		if (state != CsvDocumentReadState.DocumentHeader)
			throw new InvalidOperationException("CSV document header has already been read.");

		try
		{
			TransactionsCsvHeader header = await TransactionsCsvHeader.Parse(csvReader);
			state = CsvDocumentReadState.HeaderRow;
			return header;
		}
		catch (DocumentLoadException)
		{
			state = CsvDocumentReadState.Ended;
			throw;
		}
		catch (Exception ex)
		{
			state = CsvDocumentReadState.Ended;
			throw new DocumentLoadException("Failed to read transactions CSV document.", ex);
		}
	}

	public async IAsyncEnumerable<TransactionRecord> ReadTransactions()
	{
		if (state == CsvDocumentReadState.HeaderRow)
			columnHeaders = ReadHeaderRow();

		if (state != CsvDocumentReadState.DataRow)
			throw new InvalidOperationException("CSV document is not in a valid state to read transactions.");

		while (await csvReader.ReadAsync())
		{
			string[] fields = csvReader.Parser.Record ?? [];
			int lineNumber = csvReader.Parser.RawRow;

			if (fields.Length != columnHeaders.Length)
				throw new InvalidCsvRecordException(lineNumber, $"CSV line {lineNumber} has {fields.Length} columns, but {columnHeaders.Length} were expected.");

			yield return ParseTransactionRecord(fields, lineNumber);
		}

		state = CsvDocumentReadState.Ended;
	}

	private string[] ReadHeaderRow()
	{
		string[] headers = csvReader.Parser.Record ?? [];

		if (headers.Length == 0)
			throw new DocumentLoadException("CSV header line is empty.");

		state = CsvDocumentReadState.DataRow;

		return headers;
	}

	private static TransactionRecord ParseTransactionRecord(IReadOnlyList<string> fields, int lineNumber)
	{
		try
		{
			return new TransactionRecord
			{
				Description = fields[0],
				Project = fields[1],
				Amount = fields[2],
				Status = fields[3],
				Date = DateTime.ParseExact(fields[4], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
			};
		}
		catch (Exception ex)
		{
			throw new InvalidCsvRecordException(lineNumber, $"Invalid CSV record at line {lineNumber}.", ex);
		}
	}

	public void Dispose()
	{
		csvReader?.Dispose();
	}
}