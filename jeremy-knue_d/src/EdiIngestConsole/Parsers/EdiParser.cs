using EdiFabric;
using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.ErrorContexts;
using EdiFabric.Framework.Readers;
using EdiIngestConsole.Models;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace EdiIngestConsole.Parsers;

/// <summary>
/// Parse 837 EDI file
/// </summary>
/// <remarks>
/// License key for EdiFabric expected to exist in environment variable named "EDI_KEY"
/// </remarks>
public class EdiParser : IEdiParser
{
    protected X12ReaderSettings _defaultX12ReaderSettings = new()
    {
        ContinueOnError = true,
        LeaveOpen = true, // don't close stream, leave open so it can be read again to create hash
    };

    protected const string DefaultTemplateName = "EdiFabric.Templates.Hipaa";

    protected const string Custom837PTemplateName = "EdiIngestConsole";

    public EdiParser()
    {
        string? licenseKey = Environment.GetEnvironmentVariable("EDI_KEY");
        License.SetSerial(licenseKey);
    }

    public async Task<ParseResult> ParseFileProviderAsync(IFileProvider fileProvider)
    {
        ArgumentNullException.ThrowIfNull(fileProvider);

        using var stream = fileProvider.GetStream(true);
        return await this.ParseStreamAsync(stream, fileProvider.GetFileName());
    }


    public ParseResult ParseFileProvider(IFileProvider fileProvider)
    {
        ArgumentNullException.ThrowIfNull(fileProvider);
        using var stream = fileProvider.GetStream(false);
        return this.ParseStream(stream, fileProvider.GetFileName());
    }



    public async Task<ParseResult> ParseStreamAsync(Stream stream, string? filename = null)
    {
        ConcurrentQueue<IEdiItem> ediItems = [];

        string template = DefaultTemplateName;

        if (filename is not null &&
            Path.GetFileNameWithoutExtension(filename).Contains("837-custom-snip7")
            )
            template = Custom837PTemplateName;

        using (var ediReader = new X12Reader(stream, template, this._defaultX12ReaderSettings))
        {
            while (await ediReader.ReadAsync())
                ediItems.Enqueue(ediReader.Item);
        }

        var parseResult = this.CreateParseResultFromEdiItems(ediItems);
        parseResult.SourceHash = CreateHashFromStream(stream);

        if (!String.IsNullOrWhiteSpace(filename))
            parseResult.SourceFileName = filename;

        // for demonstrating that we have a custom class for SNIP7
        // dump the ExtraPayload value from instance of custom BHT
        // when using custom template
        if (template.Equals(Custom837PTemplateName))
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Used custom template for 837 and BHT");
           
             string extraPayload = ediItems.OfType<TS837PCustom>()?
                                      .FirstOrDefault()?
                                      .BHT_BeginningOfHierarchicalTransaction?
                                      .ExtraPayload ?? "{NOT FOUND}";

            Console.WriteLine($"Extra Payload: {extraPayload}");
            Console.ResetColor();
        }

        return parseResult;

    }


    public ParseResult ParseStream(Stream stream, string? filename = null)
    {
        List<IEdiItem>? ediItems;

        using (var ediReader = new X12Reader(stream, DefaultTemplateName, this._defaultX12ReaderSettings))
        {
            ediItems = ediReader
                        .ReadToEnd()?
                        .Where(x => x is not null)?
                        .ToList();
        }

        var parseResult = this.CreateParseResultFromEdiItems(ediItems);
        parseResult.SourceHash = CreateHashFromStream(stream);

        if (!String.IsNullOrWhiteSpace(filename))
            parseResult.SourceFileName = filename;

        return parseResult;
    }

    [Obsolete]
    public async Task<ParseResult> ParseFileAsync(string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);

        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);

        string hash = CreateHash(filename);

        ParseResult result;

        using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            result = await this.ParseStreamAsync(stream);

        if (result is null)
            throw new Exception("Null ParseResult");

        result.SourceFileName = filename;
        result.SourceHash = hash;

        return result;
    }

    [Obsolete]
    public ParseResult ParseFile(string filename)
    {
        ArgumentNullException.ThrowIfNull(filename);

        if (!File.Exists(filename))
            throw new FileNotFoundException(filename);

        string hash = CreateHash(filename);

        ParseResult result;

        using (var stream = File.OpenRead(filename))
            result = this.ParseStream(stream);

        if (result is null)
            throw new Exception("Null ParseResult");

        result.SourceFileName = filename;
        result.SourceHash = hash;

        return result;
    }


    private static string CreateHash(string filename)
    {
        using Stream stream = File.OpenRead(filename);
        return CreateHashFromStream(stream);
    }

    private static string CreateHashFromStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        stream.Position = 0;
        byte[] hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash);
    }

    protected ParseResult CreateParseResultFromEdiItems(IEnumerable<IEdiItem>? ediItems)
    {
        if (ediItems is null)
        {
            return new()
            {
                Success = false,
                Message = "Parse returned null EdiItems"
            };
        }

        var errors = ediItems.OfType<ReaderErrorContext>();
        if (errors.Any())
        {
            StringBuilder builder = new();
            Console.WriteLine($"Errors detected while parsing file.  Count of errors: {errors.Count()}");
            foreach (var err in errors.Where(x => x is not null))
            {
                if (builder.Length > 0)
                    builder.Append('\n');

                builder.Append(err.Message);

            }

            builder.Insert(0, "Parse errors detected: ");

            string errorSummary = builder.ToString();

            return new()
            {
                Success = false,
                Message = errorSummary,
                EdiItems = ediItems.ToList(),
            };
        }

        //no issues detected assume success
        return new()
        {
            Success = true,
            Message = String.Empty,
            EdiItems = ediItems.ToList(),
        };
    }

    
}


public interface IEdiParser
{
    [Obsolete]
    ParseResult ParseFile(string filename);

    [Obsolete]
    Task<ParseResult> ParseFileAsync(string filename);

    Task<ParseResult> ParseStreamAsync(Stream stream, string? filename = null);

    ParseResult ParseStream(Stream stream, string? filename = null);

    Task<ParseResult> ParseFileProviderAsync(IFileProvider fileProvider);

    ParseResult ParseFileProvider(IFileProvider fileProvider);
}