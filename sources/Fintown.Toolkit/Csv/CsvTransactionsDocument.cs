using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace DustInTheWind.Fintown.Toolkit.Csv;

internal sealed class CsvTransactionsDocument : IDisposable
{
	private readonly CsvReader csvReader;
	private CsvDocumentReadState state;
	private string[] columnHeaders;

	public CsvTransactionsDocument(TextReader textReader)
	{
		if (textReader == null) throw new ArgumentNullException(nameof(textReader));

		CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
		{
			HasHeaderRecord = false,
			IgnoreBlankLines = true
		};

		csvReader = new(textReader, csvConfiguration);
	}

	public async Task<CsvDocumentHeader> ReadDocumentHeader()
	{
		if (state != CsvDocumentReadState.DocumentHeader)
			throw new InvalidOperationException("CSV document header has already been read.");

		try
		{
			CsvDocumentHeader header = await CsvDocumentHeader.Parse(csvReader);
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

	public async Task<string[]> ReadHeaderRow()
	{
		while (await csvReader.ReadAsync())
		{
			string[] values = csvReader.Parser.Record;

			if (values == null || values.Length == 0)
				continue;

			if (values.Length != 5)
				throw new DocumentLoadException($"CSV header line has {values.Length} columns, but 5 were expected.");

			state = CsvDocumentReadState.DataRow;

			return values;
		}

		throw new DataHeaderMissingException();
	}

	public async IAsyncEnumerable<TransactionRecord> ReadTransactions()
	{
		if (state == CsvDocumentReadState.HeaderRow)
			columnHeaders = await ReadHeaderRow();

		if (state != CsvDocumentReadState.DataRow)
			throw new InvalidOperationException("CSV document is not in a valid state to read transactions.");

		while (await csvReader.ReadAsync())
		{
			string[] fields = csvReader.Parser.Record ?? [];
			int lineNumber = csvReader.Parser.RawRow;

			if (fields.Length != columnHeaders.Length)
				throw new InvalidCsvRecordLengthException(lineNumber, columnHeaders.Length, fields.Length);

			yield return ParseTransactionRecord(fields, lineNumber);
		}

		state = CsvDocumentReadState.Ended;
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
			throw new InvalidCsvRecordException(lineNumber, ex);
		}
	}

	public void Dispose()
	{
		csvReader?.Dispose();
	}
}