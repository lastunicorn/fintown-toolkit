using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using DustInTheWind.Fintown.Toolkit.Csv.Steps;
using DustInTheWind.Fintown.Toolkit.Infrastructure;

namespace DustInTheWind.Fintown.Toolkit.Csv;

internal static class CsvTransactionsDocument
{
	public static async Task<TransactionsDocument> ParseAsync(TextReader textReader)
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

		StateMachine<CsvReadStep, CsvReadContext> machine = new([
			new ReadDocumentHeaderState(),
			new ReadColumnHeaderState(),
			new ReadDataRowsState()
		])
		{
			InitialState = CsvReadStep.DocumentHeader
		};

		machine.Start(context);

		while (machine.CurrentState != null)
		{
			await machine.MoveNextAsync();
		}
		
		return document;
	}
}