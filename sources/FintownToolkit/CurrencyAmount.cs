using System.Globalization;

namespace DustInTheWind.FintownToolkit;

public readonly record struct CurrencyAmount(decimal Value, string Currency)
{
	public static CurrencyAmount Parse(string text)
	{
		if (text is null)
			throw new ArgumentNullException(nameof(text));

		string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length != 2)
			throw new FormatException($"Invalid amount format: '{text}'.");

		if (!decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
			throw new FormatException($"Invalid numeric amount: '{parts[0]}'.");

		return new CurrencyAmount(value, parts[1].ToUpperInvariant());
	}

	public static bool TryParse(string text, out CurrencyAmount amount)
	{
		amount = default;

		if (string.IsNullOrWhiteSpace(text))
			return false;

		string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		if (parts.Length != 2)
			return false;

		if (!decimal.TryParse(parts[0], NumberStyles.Number, CultureInfo.InvariantCulture, out decimal value))
			return false;

		amount = new CurrencyAmount(value, parts[1].ToUpperInvariant());
		return true;
	}

	public override string ToString()
	{
		return $"{Value.ToString("0.00", CultureInfo.InvariantCulture)} {Currency}";
	}

	public static implicit operator CurrencyAmount(string text)
	{
		return Parse(text);
	}

	public static implicit operator string(CurrencyAmount amount)
	{
		return amount.ToString();
	}
}