using DotNetEnv;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;

namespace EdiIngestConsole;

/// <summary>
/// Main worker program.
/// </summary>
internal class Program
{
    static async Task Main(string[] args)
    {
        Env.Load();

        using var serverProcess = RunServerStartupBatchFile();

        Console.WriteLine(Environment.GetEnvironmentVariable("GREET"));
        Console.ReadKey();

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

        // where we are putting files that we get from s3 
        string targetDirectory = @".\from-s3";

        await fileDownloader.DownloadFiles(targetDirectory);

        var files = Directory.GetFiles(targetDirectory);

        if (files is null || !files.Any())
        {
            Console.WriteLine("No files found");
            Console.WriteLine("Press any key to exit");
            Environment.Exit(0);
        }

        Console.WriteLine($"Found {files.Length} files to process");

        foreach (var filename in files)
        {

            Console.WriteLine($"Start processing file: {filename}");

            try
            {
                var result = await parser.ParseFileAsync(filename);

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

        }



        Console.WriteLine("You can now close the moto-server window to stop the server");
        Console.WriteLine("Press any key to exit");
        Console.ReadKey();
        Environment.Exit(0);
    }


    /// <summary>
    /// Runs batch file to start moto-server and setup s3 files
    /// </summary>
    private static Process? RunServerStartupBatchFile()
    {
        string batchFileLocation = @".\Scripts\start-moto-and-setup-s3.bat";

        if (String.IsNullOrWhiteSpace(batchFileLocation))
            throw new Exception("Startup batch file location required");

        if (!File.Exists(batchFileLocation))
            throw new FileNotFoundException(batchFileLocation);

        string filename = Path.GetFileName(batchFileLocation);

        ProcessStartInfo psi = new(filename)
        {
            CreateNoWindow = false,
            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Normal,
            WorkingDirectory = Path.GetFullPath(Path.GetDirectoryName(batchFileLocation)!),
        };

        return Process.Start(psi);
    }

}
