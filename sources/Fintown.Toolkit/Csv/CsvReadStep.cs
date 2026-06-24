namespace DustInTheWind.Fintown.Toolkit.Csv;

internal enum CsvReadStep
{
	DocumentHeader,
	ColumnHeader,
	DataRows
}