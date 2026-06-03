namespace DustInTheWind.FintownToolkit;

public class TransactionRecord
{
	public TransactionDescription Description { get; init; }

	public string Project { get; init; }

	public CurrencyAmount Amount { get; init; }

	public TransactionStatus Status { get; init; }

	public DateTime Date { get; init; }
}