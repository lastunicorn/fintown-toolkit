namespace DustInTheWind.Fintown.Toolkit.Csv;

internal enum CsvDocumentReadState
{
	DocumentHeader = 0,
	HeaderRow,
	DataRow,
	Ended
}