using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.X12;
using EdiFabric.Templates.Hipaa5010;
using EdiIngestConsole.Models;
using Microsoft.EntityFrameworkCore;
using EdiIngestConsole.Validations;

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

   
        if (ShouldSaveEdiItems(parseResult, duplcateFile))
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

    /// <summary>
    /// Should EDI Items be saved
    /// </summary>
    /// <remarks>
    /// Should only save EDI items when
    ///  1) Is not a duplicate
    ///  2) Parse successful
    ///  3) There is at least one non null EDI item
    ///  4) There are no validation results with errors
    /// </remarks>
    public static bool ShouldSaveEdiItems(ParseResult parseResult, bool isDuplicateFile)
    {
        if (isDuplicateFile)
            return false;

        return (
            parseResult.Success &&
            parseResult.EdiItems is not null &&
            parseResult.EdiItems.Any(x => x is not null) &&
            !(parseResult.ValidationResults ?? []).Any(x => !x.IsValid())
            );

    }
}

public interface IDataService
{
    Task<int> SaveItems(ParseResult parseResult);
}