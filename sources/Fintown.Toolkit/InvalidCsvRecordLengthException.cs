namespace DustInTheWind.Fintown.Toolkit;

public class InvalidCsvRecordLengthException : DocumentLoadException
{
	public InvalidCsvRecordLengthException(int lineNumber, int expectedLength, int actualLength)
		: base($"CSV line {lineNumber} has {actualLength} columns, but {expectedLength} were expected.")
	{
	}
}
public class DataHeaderMissingException : DocumentLoadException
{
	public DataHeaderMissingException()
		: base("CSV data header line is missing.")
	{
	}
}