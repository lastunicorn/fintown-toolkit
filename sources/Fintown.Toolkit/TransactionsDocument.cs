using DustInTheWind.Fintown.Toolkit.Csv;

namespace DustInTheWind.Fintown.Toolkit;

public class TransactionsDocument
{
	public DateOnly DateFrom { get; set; }

	public DateOnly DateTo { get; set; }

	public List<TransactionRecord> Transactions { get; } = [];

	public static async Task<TransactionsDocument> LoadFromFileAsync(string filePath)
	{
		if (filePath == null) throw new ArgumentNullException(nameof(filePath));

		if (string.IsNullOrWhiteSpace(filePath))
			throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

		try
		{
			using StreamReader streamReader = File.OpenText(filePath);
			return await LoadAsync(streamReader);
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
	
	public static async Task<TransactionsDocument> LoadAsync(StreamReader streamReader)
	{
		if (streamReader == null) throw new ArgumentNullException(nameof(streamReader));
		
		try
		{
			TransactionsDocument document = new();
			TransactionsCsvDocument transactionsCsvDocument = new(streamReader);

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
			throw new DocumentLoadException("Failed to load transactions CSV.", ex);
		}
	}
}