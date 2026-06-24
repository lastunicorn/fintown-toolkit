namespace DustInTheWind.Fintown.Toolkit;

public readonly struct TransactionStatus : IEquatable<TransactionStatus>
{
	public static readonly TransactionStatus Completed = new("Completed");

	public string Value { get; }

	public TransactionStatus(string value)
	{
		Value = value ?? throw new ArgumentNullException(nameof(value));
	}

	public bool Equals(TransactionStatus other)
	{
		return string.Equals(Value, other.Value, StringComparison.OrdinalIgnoreCase);
	}

	public override bool Equals(object obj)
	{
		return obj is TransactionStatus other && Equals(other);
	}

	public override int GetHashCode()
	{
		return StringComparer.OrdinalIgnoreCase.GetHashCode(Value);
	}

	public override string ToString()
	{
		return Value;
	}

	public static bool operator ==(TransactionStatus left, TransactionStatus right)
	{
		return left.Equals(right);
	}

	public static bool operator !=(TransactionStatus left, TransactionStatus right)
	{
		return !left.Equals(right);
	}

	public static implicit operator TransactionStatus(string value)
	{
		return new TransactionStatus(value);
	}

	public static implicit operator string(TransactionStatus status)
	{
		return status.Value;
	}
}