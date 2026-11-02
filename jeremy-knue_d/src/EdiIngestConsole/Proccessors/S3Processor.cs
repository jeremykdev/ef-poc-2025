using Amazon.S3.Model;
using EdiIngestConsole.Data;
using EdiIngestConsole.FileManagement;
using EdiIngestConsole.Parsers;
using EdiIngestConsole.Validations;
using System.Diagnostics;

namespace EdiIngestConsole.Proccessors;

public class S3Processor : BaseProcessor, IProcessor
{
    public S3Processor(IEdiParser parser, IDataService dataService, IFileDownloader fileDownloader, IValidator validator) : base(parser, dataService, fileDownloader, validator)
    {

    }

    public async Task Process()
    {
        try
        {
            using var serverProcess = RunServerStartupBatchFile();

            Console.WriteLine("Starting application, ensure that moto server scripts are running and S3 setup has completed before pressing Enter to continue.");
            Console.ReadLine();

            List<S3Object> s3Items = [];

            try
            {
                s3Items = await this.FileDownloader.List3Objects();

                if (s3Items is null || s3Items.Count == 0)
                {
                    Console.WriteLine("No files found on server");
                    return;
                }
            }
            catch (Exception ex)
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
                    var stream = await this.FileDownloader.GetStream(file.Key);

                    //  await this.ProcessFile(new LocalFileProvider(file));
                    var fi = new StreamFileProvider(stream, file.Key);
                    await this.ProcessFile(fi);

                    stream?.Dispose();
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
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while processing. Message: {ex.Message}");
        }
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

        return System.Diagnostics.Process.Start(psi);
    }
}
