using System;
using Microsoft.EntityFrameworkCore;
using EdiIngestConsole.Models;
using EdiFabric.Templates.Hipaa5010;
using EdiFabric.Core.Model.Edi.X12;

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

    public DbSet<TS837P> TS837PEntities { get; set; }

    public DbSet<GE> GeEntities { get; set; }

    public DbSet<IEA> IeaEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GE>().Property<int>("Id").UseIdentityColumn();
        modelBuilder.Entity<IEA>().Property<int>("Id").UseIdentityColumn();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONN"));
        }
    }

}