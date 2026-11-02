using EdiFabric.Core.Model.Edi;
using EdiIngestConsole.Data;
using EdiIngestConsole.FileManagement;
using EdiIngestConsole.Parsers;
using EdiIngestConsole.Validations;

namespace EdiIngestConsole.Proccessors;

public abstract class BaseProcessor
{
    protected readonly IEdiParser Parser;
    protected readonly IDataService DataService;
    protected readonly IFileDownloader FileDownloader;
    protected readonly IValidator Validator;

    protected BaseProcessor(
        IEdiParser parser,
        IDataService dataService,
        IFileDownloader fileDownloader,
        IValidator validator
    )
    {
        this.Parser = parser;
        this.DataService = dataService;
        this.FileDownloader = fileDownloader;
        this.Validator = validator;
    }

     

    public async Task ProcessFile(IFileProvider fileProvider)
    {        
        ParseResult result = await this.Parser.ParseFileProviderAsync(fileProvider);

        if (result is null)
        {
            Console.WriteLine("Parse result is null");
            return;
        }

        this.Validate(result);

        await this.Save(result);

    }

    public async Task<int> Save(ParseResult parseResult)
    {
        Console.WriteLine("Preparing to save data");
        int count = await this.DataService!.SaveItems(parseResult);

        Console.WriteLine($"Number of items saved: {count}");
        return count;
    }


    public void Validate(ParseResult result)
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
                validationResult = this.Validator.Validate(item as EdiMessage);

                result.ValidationResults.Add(validationResult);

                var consoleColor = validationResult.IsValid() ? ConsoleColor.Green : ConsoleColor.Red;
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(validationResult.ToString('\n'));
                Console.ResetColor();
            }
            else if (item is ControlSegment)
            {
                Console.WriteLine($"validating ControlSegment {item.GetType()}...");
                validationResult = this.Validator.Validate(item as ControlSegment);

                result.ValidationResults.Add(validationResult);

                var consoleColor = validationResult.IsValid() ? ConsoleColor.Green : ConsoleColor.Red;
                Console.ForegroundColor = consoleColor;
                Console.WriteLine(validationResult.ToString('\n'));
                Console.ResetColor();
            }
        }

        Console.WriteLine("");
        Console.WriteLine("");

    }

}
