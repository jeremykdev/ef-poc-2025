using DotNetEnv;
using EdiIngestConsole;
using Microsoft.Extensions.DependencyInjection;

namespace EdiIngestConsole;

/// <summary>
/// Main worker program.
/// </summary>
internal class Program
{
    static async Task Main(string[] args)
    {
        Env.Load();

        Console.WriteLine(Environment.GetEnvironmentVariable("GREET"));

        // setup dependency injection
        ServiceCollection services = new();
        services.AddTransient<EdiDbContext>(); // note that connection string set in OnConfiguring
        services.AddTransient<IEdiParser, EdiParser>();
        services.AddTransient<IDataService, DataService>();
        services.AddTransient<IFileMover, FileMover>();
        services.AddTransient<IFileDownloader, AwsFileDownloader>();

        var provider = services.BuildServiceProvider();

        IEdiParser parser = provider.GetService<IEdiParser>()!;
        IDataService dataService = provider.GetService<IDataService>()!;
        IFileMover fileMover = provider.GetService<IFileMover>()!;
        IFileDownloader fileDownloader = provider.GetService<IFileDownloader>()!;


        // TODO: uncomment below and refactor
        /*


        // provide file name to be processed        
        string filename = "837.txt"; // "837-malformed.txt"; // "Empty.txt"; // "837.txt";

        Console.WriteLine($"Start processing file: {filename}");

        try
        {
            var result = parser.ParseFile(filename);

            if (result is null)
                Console.WriteLine("Parse result is null");
            else
            {
                Console.WriteLine($"{result.EdiItems.Count} items parsed.");
                Console.WriteLine($"Hash: {result.SourceHash}");
                foreach (var item in result.EdiItems)
                    Console.WriteLine(item);

                Console.WriteLine("");
                Console.WriteLine("");

                Console.WriteLine("Preparing to save data");
                int count = await dataService!.SaveItems(result);

                Console.WriteLine($"Number of items saved: {count}");

                Console.WriteLine("Moving file");
                string moved = fileMover.MoveCompletedFile(filename);
                Console.WriteLine($"File moved to {moved}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }


        */

        Console.WriteLine("Press any key to exit.");



        Console.ReadKey();
    }
}
