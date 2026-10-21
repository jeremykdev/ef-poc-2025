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

        var provider = services.BuildServiceProvider();

        IEdiParser parser = provider.GetService<IEdiParser>()!;
        IDataService dataService = provider.GetService<IDataService>()!;
        IFileMover fileMover = provider.GetService<IFileMover>()!;

        string filename = "837.txt";

        var result = parser.ParseFile(filename);

        if (result is null)
            Console.WriteLine("Parse result is null");
        else if (!result.Success)
            Console.WriteLine($"Parse failed {result.Message}");
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

        Console.WriteLine("Press any key to exit.");

        Console.ReadKey();
    }
}
