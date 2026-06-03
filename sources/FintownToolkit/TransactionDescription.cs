namespace DustInTheWind.FintownToolkit;

public readonly struct TransactionDescription : IEquatable<TransactionDescription>
{
	public static readonly TransactionDescription InvestingFunds = new("Investing funds");
	public static readonly TransactionDescription DepositFunds = new("Deposit funds");
	public static readonly TransactionDescription InterestPayOut = new("Interest Pay-Out");

	public string Value { get; }

	public TransactionDescription(string value)
	{
		if (value is null)
			throw new ArgumentNullException(nameof(value));

		Value = value;
	}

	public bool Equals(TransactionDescription other)
	{
		return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
	}

	public override bool Equals(object obj)
	{
		return obj is TransactionDescription other && Equals(other);
	}

	public override int GetHashCode()
	{
		return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
	}

	public override string ToString()
	{
		return Value;
	}

	public static bool operator ==(TransactionDescription left, TransactionDescription right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TransactionDescription left, TransactionDescription right)
	{
		return !left.Equals(right);
	}

	public static implicit operator TransactionDescription(string value)
	{
		return new TransactionDescription(value);
	}

	public static implicit operator string(TransactionDescription description)
	{
		return description.Value;
	}
}
