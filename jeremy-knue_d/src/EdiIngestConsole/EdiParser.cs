using EdiFabric;
using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.ErrorContexts;
using EdiFabric.Framework.Readers;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;

namespace EdiIngestConsole;

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
    };

    protected const string TemplateName = "EdiFabric.Templates.Hipaa";

    public EdiParser()
	{
		string? licenseKey = Environment.GetEnvironmentVariable("EDI_KEY");
        License.SetSerial(licenseKey);
    }

    public async Task<ParseResult> ParseStreamAsync(Stream stream)
    {
        ConcurrentQueue<IEdiItem> ediItems = [];

        using (var ediReader = new X12Reader(stream, TemplateName, this._defaultX12ReaderSettings))
        {
            while (await ediReader.ReadAsync())
                ediItems.Enqueue(ediReader.Item);            
        }

        return this.CreateParseResultFromEdiItems(ediItems?.ToList());
    }

    /*
    public async Task<ParseResult> ParseStreamAsync(Stream stream)
    {
        List<IEdiItem>? ediItems;
        
        using (var ediReader = new X12Reader(stream, TemplateName, this._defaultX12ReaderSettings))
        {
            ediItems = (await ediReader.ReadToEndAsync())?
                        .Where(x => x is not null)?
                        .ToList();            
        }

        return this.CreateParseResultFromEdiItems(ediItems);
    }
    */


    public ParseResult ParseStream(Stream stream)
    {
       List<IEdiItem>? ediItems;

        using (var ediReader = new X12Reader(stream, TemplateName, this._defaultX12ReaderSettings))
        {
            ediItems = ediReader
                        .ReadToEnd()?
                        .Where(x => x is not null)?
                        .ToList();
        }

        return this.CreateParseResultFromEdiItems(ediItems);
    }

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
            Message= String.Empty,
            EdiItems = ediItems.ToList(),
        };
    }

}


public interface IEdiParser
{
    ParseResult ParseFile(string filename);
    Task<ParseResult> ParseFileAsync(string filename);
}