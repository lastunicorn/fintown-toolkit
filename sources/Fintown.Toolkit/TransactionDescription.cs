namespace DustInTheWind.Fintown.Toolkit;

public sealed record class TransactionDescription
{
	public static readonly TransactionDescription InvestingFunds = new("Investing funds");
	public static readonly TransactionDescription DepositFunds = new("Deposit funds");
	public static readonly TransactionDescription InterestPayOut = new("Interest Pay-Out");

	private static readonly Dictionary<string, TransactionDescription> KnownValues = new(StringComparer.Ordinal)
	{
		[InvestingFunds.Value] = InvestingFunds,
		[DepositFunds.Value] = DepositFunds,
		[InterestPayOut.Value] = InterestPayOut
	};

	public string Value { get; }

	public TransactionDescription(string value)
	{
		Value = value ?? throw new ArgumentNullException(nameof(value));
	}

	public override string ToString()
	{
		return Value;
	}

	public static implicit operator TransactionDescription(string value)
	{
		return value == null
			? null
			: new TransactionDescription(value);
	}

	public static implicit operator string(TransactionDescription description)
	{
		return description?.Value;
	}
}