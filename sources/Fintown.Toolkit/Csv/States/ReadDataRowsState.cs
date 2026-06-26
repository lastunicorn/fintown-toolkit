using System.Globalization;
using DustInTheWind.Machina;

namespace DustInTheWind.Fintown.Toolkit.Csv.States;

internal class ReadDataRowsState : IState<CsvReadState, CsvReadContext>
{
	public CsvReadState Id => CsvReadState.DataRows;

	public async Task<CsvReadState?> ExecuteAsync(CsvReadContext context)
	{
		while (await context.CsvReader.ReadAsync())
		{
			string[] fields = context.CsvReader.Parser.Record ?? [];
			int lineNumber = context.CsvReader.Parser.RawRow;

			if (fields.Length != context.ColumnHeaders.Length)
				throw new InvalidCsvRecordLengthException(lineNumber, context.ColumnHeaders.Length, fields.Length);

			context.Document.Add(ParseTransactionRecord(fields, lineNumber));
		}

		return null;
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
}