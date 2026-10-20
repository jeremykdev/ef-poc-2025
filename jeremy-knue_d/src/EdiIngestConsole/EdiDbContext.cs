using System;
using Microsoft.EntityFrameworkCore;
using EdiFabric.Templates.Hipaa5010;
using EdiFabric.Core.Model.Edi.X12;

namespace EdiIngestConsole;

/// <summary>
/// EF database context
/// </summary>
public class EdiDbContext : DbContext 
{
    public EdiDbContext(DbContextOptions<EdiDbContext> options) : base(options)
    {

    }

    public EdiDbContext()
    {
        
    }

    public DbSet<ISA> IsaEntities { get; set; }

    public DbSet<GS> GsEntities { get; set; }

    public DbSet<TS837P> TS837PEntities { get; set; }

    public DbSet<GE> GeEntities { get; set; }

    public DbSet<IEA> IeaEntities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // add auto increment ids for entities' tables

        modelBuilder.Entity<GE>().Property<int>("GE_Id").UseIdentityColumn();
        modelBuilder.Entity<GE>().HasKey("GE_Id");
        
        modelBuilder.Entity<IEA>().Property<int>("IEA_Id").UseIdentityColumn();
        modelBuilder.Entity<IEA>().HasKey("IEA_Id");

        modelBuilder.Entity<GS>().Property<int>("GS_Id").UseIdentityColumn();
        modelBuilder.Entity<GS>().HasKey("GS_Id");

        modelBuilder.Entity<ISA>().Property<int>("ISA_Id").UseIdentityColumn();
        modelBuilder.Entity<ISA>().HasKey("ISA_Id");

        modelBuilder.Entity<TS837P>().Property<int>("TS837P_Id").UseIdentityColumn();
        modelBuilder.Entity<TS837P>().HasKey("TS837P_Id");
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // default to using database connection string from enviroment vars
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("SQL_CONN"));   
        }
    }

}