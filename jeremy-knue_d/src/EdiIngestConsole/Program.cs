using Amazon.S3.Model;
using DotNetEnv;
using EdiFabric.Core.Model.Edi;
using EdiIngestConsole.Validations;
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
        services.AddTransient<IValidator, Validator>();

        var provider = services.BuildServiceProvider();

        IEdiParser parser = provider.GetService<IEdiParser>()!;
        IDataService dataService = provider.GetService<IDataService>()!;
        IFileMover fileMover = provider.GetService<IFileMover>()!;
        IFileDownloader fileDownloader = provider.GetService<IFileDownloader>()!;
        IValidator validator = provider.GetService<IValidator>()!;

        List<S3Object> s3Items = [];

        try
        {
            s3Items = await fileDownloader.List3Objects();

            if (s3Items is null || s3Items.Count == 0)
            {
                Console.WriteLine("No files found on server");
                Console.WriteLine("Press any key to exit");
                Environment.Exit(0);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        Console.WriteLine($"Found {s3Items.Count} files to process");

        foreach (var file in s3Items)
        {
            Console.WriteLine("===========================");
            Console.WriteLine($"Start processing file/object: {file.Key}");

            try
            {                
                ParseResult result;
                using (var stream = await fileDownloader.GetStream(file.Key))
                {
                    result = await parser.ParseStreamAsync(stream, file.Key);
                }

                if (result is null)
                    Console.WriteLine("Parse result is null");
                else
                {
                    ValidationResult? validationResult = null;
                    Console.WriteLine($"{result.EdiItems.Count} items parsed.");
                    Console.WriteLine($"Hash: {result.SourceHash}");
                    foreach (var item in result.EdiItems)
                    {
                        Console.WriteLine(item);

                        if (item is EdiMessage)
                        {
                            Console.WriteLine($"validating EdiMessage {item.GetType()} ...");
                            validationResult = validator.Validate(item as EdiMessage);

                            result.ValidationResults.Add(validationResult);

                            var consoleColor = validationResult.IsValid() ? ConsoleColor.Green : ConsoleColor.Red;
                            Console.ForegroundColor = consoleColor;
                            Console.WriteLine(validationResult.ToString('\n'));
                            Console.ResetColor();
                        }
                        else if (item is ControlSegment)
                        {
                            Console.WriteLine($"validating ControlSegment {item.GetType()}...");
                            validationResult = validator.Validate(item as ControlSegment);

                            result.ValidationResults.Add(validationResult);

                            var consoleColor = validationResult.IsValid() ? ConsoleColor.Green : ConsoleColor.Red;
                            Console.ForegroundColor = consoleColor;
                            Console.WriteLine(validationResult.ToString('\n'));
                            Console.ResetColor();
                        }
                    }

                    Console.WriteLine("");
                    Console.WriteLine("");
                   
                    Console.WriteLine("Preparing to save data");
                    int count = await dataService!.SaveItems(result);

                    Console.WriteLine($"Number of items saved: {count}");
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
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
