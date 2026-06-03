using DustInTheWind.ConsoleTools.Controls;
using DustInTheWind.ConsoleTools.Controls.Tables;

namespace DustInTheWind.Fintown.Toolkit.Demo;

internal static class Program
{
	public static async Task Main(string[] args)
	{
		const string fileName = "transactions.csv";

		try
		{
			TransactionsDocument document = await TransactionsDocument.LoadFromFileAsync(fileName);

			Console.WriteLine($"Loaded {document.Transactions.Count} transactions from {document.DateFrom:yyyy-MM-dd} to {document.DateTo:yyyy-MM-dd}.");
			Console.WriteLine();

			DataGrid dataGrid = Display(document);
			dataGrid.Display();
		}
		catch (DocumentLoadException ex)
		{
			await Console.Error.WriteLineAsync($"Failed to read '{fileName}': {ex.Message}");
			Environment.ExitCode = 1;
		}
		catch (Exception ex)
		{
			await Console.Error.WriteLineAsync($"Unexpected error: {ex.Message}");
			Environment.ExitCode = 1;
		}
	}

	private static DataGrid Display(TransactionsDocument document)
	{
		DataGrid dataGrid = new()
		{
			Title = $"Transactions ({document.DateFrom:yyyy-MM-dd} - {document.DateTo:yyyy-MM-dd})",
			BorderTemplate = BorderTemplate.PlusMinusBorderTemplate,
			Footer = $"Count: {document.Transactions.Count}"
		};

		dataGrid.Columns.Add("Date");
		dataGrid.Columns.Add("Description");
		dataGrid.Columns.Add("Project");
		dataGrid.Columns.Add("Amount", HorizontalAlignment.Right);
		dataGrid.Columns.Add("Status");

		foreach (TransactionRecord transaction in document.Transactions)
		{
			dataGrid.Rows.Add(
				transaction.Date.ToString("yyyy-MM-dd HH:mm:ss"),
				transaction.Description.ToString(),
				transaction.Project,
				transaction.Amount.ToString(),
				transaction.Status.ToString());
		}

		return dataGrid;
	}
}