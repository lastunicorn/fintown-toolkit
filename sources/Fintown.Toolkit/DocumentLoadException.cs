namespace DustInTheWind.Fintown.Toolkit;

public class DocumentLoadException : Exception
{
	public DocumentLoadException(string message)
		: base(message)
	{
	}

	public DocumentLoadException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}