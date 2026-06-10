using DustInTheWind.Fintown.Toolkit.Csv;

namespace DustInTheWind.Fintown.Toolkit;

public class TransactionsDocument
{
	public DateOnly DateFrom { get; set; }

	public DateOnly DateTo { get; set; }

	public List<TransactionRecord> Transactions { get; } = [];

	public static async Task<TransactionsDocument> LoadFromFileAsync(string filePath)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		try
		{
			using StreamReader streamReader = File.OpenText(filePath);
			return await LoadInternalAsync(streamReader);
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public static async Task<TransactionsDocument> LoadAsync(string csv)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(csv);

		try
		{
			using StringReader stringReader = new(csv);
			return await LoadInternalAsync(stringReader);
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public static async Task<TransactionsDocument> LoadAsync(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream);

		try
		{
			using StreamReader streamReader = new(stream);
			return await LoadInternalAsync(streamReader);
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public static async Task<TransactionsDocument> LoadAsync(FileInfo fileInfo)
	{
		ArgumentNullException.ThrowIfNull(fileInfo);

		try
		{
			using StreamReader streamReader = fileInfo.OpenText();
			return await LoadInternalAsync(streamReader);
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}

	public static Task<TransactionsDocument> LoadAsync(StreamReader streamReader)
	{
		ArgumentNullException.ThrowIfNull(streamReader);

		return LoadInternalAsync(streamReader);
	}

	public static Task<TransactionsDocument> LoadAsync(TextReader textReader)
	{
		ArgumentNullException.ThrowIfNull(textReader);

		return LoadInternalAsync(textReader);
	}

	private static async Task<TransactionsDocument> LoadInternalAsync(TextReader textReader)
	{
		try
		{
			TransactionsDocument document = new();
			CsvTransactionsDocument csvTransactionsDocument = new(textReader);

			CsvDocumentHeader header = await csvTransactionsDocument.ReadDocumentHeader();
			document.DateFrom = header.DateFrom;
			document.DateTo = header.DateTo;

			await foreach (TransactionRecord transaction in csvTransactionsDocument.ReadTransactions())
				document.Transactions.Add(transaction);

			return document;
		}
		catch (DocumentLoadException)
		{
			throw;
		}
		catch (Exception ex)
		{
			throw new DocumentLoadException(ex);
		}
	}
}