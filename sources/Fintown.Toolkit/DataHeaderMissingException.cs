namespace DustInTheWind.Fintown.Toolkit;

public class DataHeaderMissingException : DocumentLoadException
{
	public DataHeaderMissingException()
		: base("CSV data header line is missing.")
	{
	}
}