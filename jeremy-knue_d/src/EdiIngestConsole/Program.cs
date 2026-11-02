using Amazon.S3.Model;
using DotNetEnv;
using EdiFabric.Core.Model.Edi;
using EdiIngestConsole.Data;
using EdiIngestConsole.FileManagement;
using EdiIngestConsole.Parsers;
using EdiIngestConsole.Proccessors;
using EdiIngestConsole.Validations;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EdiIngestConsole;

/// <summary>
/// Main worker program.
/// </summary>
public class Program
{
    // keys for AWS S3 vs LocalFile
    private const string KeyS3 = "S3";
    private const string KeyLocalFile = "Local";

    static async Task Main(string[] args)
    {
        Env.Load();
  
        // setup dependency injection
        ServiceCollection services = new();
        services.AddTransient<EdiDbContext>(); // note that connection string set in OnConfiguring
        services.AddTransient<IEdiParser, EdiParser>();
        services.AddTransient<IDataService, DataService>();
        services.AddTransient<IFileMover, FileMover>();
        services.AddTransient<IFileDownloader, AwsFileDownloader>();
        services.AddTransient<IValidator, Validator>();
        services.AddKeyedTransient<IProcessor, S3Processor>(KeyS3);
        services.AddKeyedTransient<IProcessor, LocalFileProcessor>(KeyLocalFile);

        var provider = services.BuildServiceProvider();

        string processorKey = GetProcessorKey();
        IProcessor processor = provider.GetRequiredKeyedService<IProcessor>(processorKey);
        await processor.ProcessAsync();

        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        Environment.Exit(0);
    }

    /// <summary>
    /// Get the key to use for IProcessor DI resolution
    /// </summary>
    /// <remarks>
    /// One of the key values for AWS S3 vs LocalFile expected to exist in environment variable "PROCESSOR_KEY"
    /// If not found will default to LocalFile 
    /// </remarks>
    private static string GetProcessorKey()
    {
        string envValue = Environment.GetEnvironmentVariable("PROCESSOR_KEY") ?? String.Empty;


        if (envValue == KeyLocalFile)
            return KeyLocalFile;

        if (envValue == KeyS3)
            return KeyS3;

        Console.WriteLine($"PROCESSOR_KEY {envValue} does not match any valid keys.  Will use default");
        return KeyLocalFile;
    }

}
