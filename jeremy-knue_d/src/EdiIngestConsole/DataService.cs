using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.X12;
using EdiFabric.Templates.Hipaa5010;
using EdiIngestConsole.Models;
using Microsoft.EntityFrameworkCore;

namespace EdiIngestConsole;

/// <summary>
/// Data service wraps EF db context with logic to handle IEdiItems
/// Also handles migrations
/// </summary>
public class DataService(EdiDbContext dbContext) : IDataService
{
    public async Task<int> SaveItems(ParseResult parseResult)
    {
        ArgumentNullException.ThrowIfNull(parseResult);

        if (dbContext.Database is not null)
            await dbContext.Database.MigrateAsync();

        History history = new(parseResult);

        // check for duplicates
        bool duplcateFile = false;
        if (!String.IsNullOrWhiteSpace(parseResult.SourceHash))
        {
            duplcateFile = dbContext.HistoryEntities
                                .Where(x => x.SourceHash.Equals(parseResult.SourceHash))
                                .Any();

            if (duplcateFile)
            {
                Console.WriteLine("Duplicate detected. Records from file will not be saved");
                history.Message = "Duplicate file detected";
            }
        }

        // only save items from a valid parse, avoid partial processing of malformed file
        if (parseResult.Success && parseResult.EdiItems is not null && !duplcateFile)
        {
            foreach (var item in parseResult.EdiItems)
            {
                if (item is null)
                    continue;

                if (item is ISA)
                    dbContext.Add((item as ISA)!);

                if (item is GS)
                    dbContext.Add((item as GS)!);

                if (item is TS837P)
                    dbContext.Add((item as TS837P)!);

                if (item is GE)
                    dbContext.Add((item as GE)!);

                if (item is IEA)
                    dbContext.Add((item as IEA)!);
            }
        }

        dbContext.Add(history);

        int result = await dbContext.SaveChangesAsync();
        return result;
    }
}

public interface IDataService
{
    Task<int> SaveItems(ParseResult parseResult);
}