using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.X12;
using EdiFabric.Templates.Hipaa5010;
using Microsoft.EntityFrameworkCore;

namespace EdiIngestConsole;

public class DataService(EdiDbContext dbContext) : IDataService
{
    public async Task<int> SaveItems(IEnumerable<IEdiItem> ediItems)
    {
        if (ediItems is null || !ediItems.Any())
            return 0;
      
        if (dbContext.Database is not null)
            await dbContext.Database.MigrateAsync();

        foreach (var item in ediItems)
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

        int result = await dbContext.SaveChangesAsync();
        return result;
    }
}

public interface IDataService
{
    Task<int> SaveItems(IEnumerable<IEdiItem> ediItems);
}