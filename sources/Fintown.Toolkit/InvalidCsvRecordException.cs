namespace DustInTheWind.Fintown.Toolkit;

public class InvalidCsvRecordException : DocumentLoadException
{
	public int LineNumber { get; }

	public InvalidCsvRecordException(int lineNumber, string message)
		: base(message)
	{
		LineNumber = lineNumber;
	}

	public InvalidCsvRecordException(int lineNumber, string message, Exception innerException)
		: base(message, innerException)
	{
		LineNumber = lineNumber;
	}
}

