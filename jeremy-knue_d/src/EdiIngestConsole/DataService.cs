using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.X12;
using EdiFabric.Templates.Hipaa5010;
using EdiIngestConsole.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            if (item is null)
                continue;

            if (item is ISA)
            {
                IsaEntity isaEntity = new (item as ISA);
                dbContext.Add(isaEntity);
            }

            if (item is GS)
            {
                GsEntity gsEntity = new (item as GS);
                dbContext.Add(gsEntity);
            }

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