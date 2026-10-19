using System;
using Microsoft.EntityFrameworkCore;
using EdiIngestConsole.Models;

namespace EdiIngestConsole;

public interface IEdiDbContext
{
    DbSet<IsaEntity> IsaEntities { get; set; }
}

public class EdiDbContext : DbContext, IEdiDbContext
{
    public EdiDbContext(DbContextOptions<EdiDbContext> options) : base(options)
    {

    }

    public EdiDbContext()
    {
        
    }

    public DbSet<IsaEntity> IsaEntities { get; set; }

    public DbSet<GsEntity> GsEntities { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONN"));
        }
    }

}