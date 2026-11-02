using EdiFabric.Core.Model.Edi;
using EdiIngestConsole.Data;
using EdiIngestConsole.FileManagement;
using EdiIngestConsole.Parsers;
using EdiIngestConsole.Validations;

namespace EdiIngestConsole.Proccessors;

public class LocalFileProcessor : BaseProcessor, IProcessor
{
    protected const string SourceFolder = @".\EdiFiles";

    private readonly IFileMover _fileMover;

    public LocalFileProcessor(IEdiParser parser, IDataService dataService, IFileDownloader fileDownloader, IValidator validator, IFileMover fileMover) : base(parser, dataService, fileDownloader, validator)
    {
        this._fileMover = fileMover;
    }

    public async Task ProcessAsync()
    {
        try
        {
            var files = Directory.GetFiles(SourceFolder);

            if (files is null || !files.Any())
            {
                Console.WriteLine($"No files found in {SourceFolder}");
                return;
            }

            Console.WriteLine($"Found {files.Length} files to process");


            
            foreach (var file in files)
            {
                Console.WriteLine("===========================");
                Console.WriteLine($"Start processing file: {Path.GetFileName(file)}");


                ParseResult result = await this.Parser.ParseFileAsync(file);

                if (result is null)
                {
                    Console.WriteLine("Parse result is null");
                    continue;
                }

                this.Validate(result);

                await this.Save(result);

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
