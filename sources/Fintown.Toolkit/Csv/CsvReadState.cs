namespace DustInTheWind.Fintown.Toolkit.Csv;

internal enum CsvReadState
{
	DocumentHeader,
	ColumnsHeader,
	DataRows
}