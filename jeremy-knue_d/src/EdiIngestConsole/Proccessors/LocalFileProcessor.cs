using EdiIngestConsole.Data;
using EdiIngestConsole.FileManagement;
using EdiIngestConsole.Parsers;
using EdiIngestConsole.Validations;

namespace EdiIngestConsole.Proccessors;

public class LocalFileProcessor : BaseProcessor, IProcessor
{
    protected const string SourceFolder = @".\EdiFiles";

    private readonly IFileMover _fileMover;

    public LocalFileProcessor(IEdiParser parser, IDataService dataService,  IValidator validator, IFileMover fileMover) : base(parser, dataService, validator)
    {
        this._fileMover = fileMover;
    }

    
    public async Task Process()
    {
        try
        {
            Console.WriteLine($"Startng process for local files in {SourceFolder}");
            var files = Directory.GetFiles(SourceFolder);

            if (files is null || !files.Any())
            {
                Console.WriteLine($"No files found in {SourceFolder}");
                return;
            }

            Console.WriteLine($"Found {files.Length} files to process");
                        
            foreach (var file in files.OrderBy(x => x))
            {
                Console.WriteLine("===========================");
                Console.WriteLine($"Start processing file: {Path.GetFileName(file)}");

                await this.ProcessFile(new LocalFileProvider(file));
                                
                string destination = this._fileMover.MoveCompletedFile(file);
                Console.WriteLine($"File moved to {destination}");              
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception while processing. Message: {ex.Message}");
        }


    }
}
