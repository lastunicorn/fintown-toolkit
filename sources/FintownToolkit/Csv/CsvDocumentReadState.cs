namespace DustInTheWind.FintownToolkit.Csv;

internal enum CsvDocumentReadState
{
	DocumentHeader = 0,
	HeaderRow,
	DataRow,
	Ended
}