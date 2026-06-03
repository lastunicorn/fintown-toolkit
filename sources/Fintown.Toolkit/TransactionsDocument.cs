using DustInTheWind.Fintown.Toolkit.Csv;

namespace DustInTheWind.Fintown.Toolkit;

public class TransactionsDocument
{
	public DateOnly DateFrom { get; set; }

	public DateOnly DateTo { get; set; }

	public List<TransactionRecord> Transactions { get; } = [];

	public static Task<TransactionsDocument> LoadFromFileAsync(string filePath)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

		try
		{
			using StreamReader streamReader = File.OpenText(filePath);
			return LoadInternalAsync(streamReader);
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

	public static Task<TransactionsDocument> LoadAsync(string csv)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(csv);

		try
		{
			using StringReader stringReader = new(csv);
			return LoadInternalAsync(stringReader);
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

	public static Task<TransactionsDocument> LoadAsync(Stream stream)
	{
		ArgumentNullException.ThrowIfNull(stream);

		try
		{
			using StreamReader streamReader = new(stream);
			return LoadInternalAsync(streamReader);
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

	public static Task<TransactionsDocument> LoadAsync(FileInfo fileInfo)
	{
		ArgumentNullException.ThrowIfNull(fileInfo);

		try
		{
			using StreamReader streamReader = fileInfo.OpenText();
			return LoadInternalAsync(streamReader);
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
			TransactionsCsvDocument transactionsCsvDocument = new(textReader);

			TransactionsCsvHeader header = await transactionsCsvDocument.ReadDocumentHeader();
			document.DateFrom = header.DateFrom;
			document.DateTo = header.DateTo;

			await foreach (TransactionRecord transaction in transactionsCsvDocument.ReadTransactions())
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