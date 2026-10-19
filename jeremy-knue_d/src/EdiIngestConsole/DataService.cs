using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.X12;
using EdiIngestConsole.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdiIngestConsole;

public class DataService(EdiDbContext dbContext) : IDataService
{
    public async Task<int> SaveItems(IEnumerable<IEdiItem> ediItems)
    {
        if (ediItems is null || !ediItems.Any())
            return 0;

        await dbContext.Database.MigrateAsync();

        foreach (var item in ediItems)
        {
            if (item is ISA)
            {
                var isaEntity = new IsaEntity(item as ISA);
                dbContext.Add(isaEntity);
            }
        }

        int result = await dbContext.SaveChangesAsync();
        return result;
    }
}

public interface IDataService
{
    Task<int> SaveItems(IEnumerable<IEdiItem> ediItems);
}