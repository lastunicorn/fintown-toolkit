namespace DustInTheWind.FintownToolkit;

public class TransactionRecord
{
	public string Description { get; init; }

	public string Project { get; init; }

	public decimal Amount { get; init; }

	public string Currency { get; init; }

	public string Status { get; init; }

	public DateTime Date { get; init; }
}