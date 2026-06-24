using DustInTheWind.Fintown.Toolkit.Infrastructure;

namespace DustInTheWind.Fintown.Toolkit.Csv.Steps;

internal class ReadColumnHeaderState : IState<CsvReadStep, CsvReadContext>
{
    public CsvReadStep Id => CsvReadStep.ColumnHeader;

    public async Task<CsvReadStep?> ExecuteAsync(CsvReadContext context)
    {
        while (await context.CsvReader.ReadAsync())
        {
            string[] values = context.CsvReader.Parser.Record;

            if (values == null || values.Length == 0)
                continue;

            if (values.Length != 5)
                throw new DocumentLoadException($"CSV header line has {values.Length} columns, but 5 were expected.");

            context.ColumnHeaders = values;
            return CsvReadStep.DataRows;
        }

        throw new DataHeaderMissingException();
    }
}
