namespace DustInTheWind.Fintown.Toolkit;

public sealed record class TransactionStatus
{
	public static readonly TransactionStatus Completed = new("Completed");

	public string Value { get; }

	public TransactionStatus(string value)
	{
		Value = value ?? throw new ArgumentNullException(nameof(value));
	}

	public override string ToString()
	{
		return Value;
	}

	public static implicit operator TransactionStatus(string value)
	{
		return value == null
			? null
			: new TransactionStatus(value);
	}

	public static implicit operator string(TransactionStatus status)
	{
		return status?.Value;
	}
}