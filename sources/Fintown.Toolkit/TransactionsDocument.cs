using System.Collections.ObjectModel;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DustInTheWind.Fintown.Toolkit.Csv;
using DustInTheWind.Fintown.Toolkit.Csv.States;
using DustInTheWind.Fintown.Toolkit.Infrastructure;

namespace DustInTheWind.Fintown.Toolkit;

public class TransactionsDocument : Collection<TransactionRecord>
{
	public DateOnly DateFrom { get; set; }

	public DateOnly DateTo { get; set; }

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
			CsvConfiguration csvConfiguration = new(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = false,
				IgnoreBlankLines = true
			};

			using CsvReader csvReader = new(textReader, csvConfiguration);

			TransactionsDocument document = new();

			CsvReadContext context = new()
			{
				CsvReader = csvReader,
				Document = document
			};

			StateMachine<CsvReadState, CsvReadContext> machine = new([
				new ReadDocumentHeaderState(),
				new ReadColumnHeaderState(),
				new ReadDataRowsState()
			])
			{
				InitialState = CsvReadState.DocumentHeader
			};

			machine.Start(context);

			while (machine.CurrentState != null)
			{
				await machine.MoveNextAsync();
			}

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