using EdiFabric;
using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.ErrorContexts;
using EdiFabric.Framework.Readers;
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
	protected string? licenseKey;

	public EdiParser()
	{
		this.licenseKey = Environment.GetEnvironmentVariable("EDI_KEY");				
	}

    public ParseResult ParseStream(Stream stream)
    {
        License.SetSerial(this.licenseKey);

        List<IEdiItem> ediItems;

        X12ReaderSettings x12ReaderSettings = new()
        {
            ContinueOnError = true,
        };
       
        using (var ediReader = new X12Reader(stream, "EdiFabric.Templates.Hipaa", x12ReaderSettings))
            ediItems = ediReader.ReadToEnd().Where(x => x is not null).ToList();       

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
                EdiItems = ediItems
            };
        }

        return new()
        {
            Success = true,
            Message = String.Empty, // no message needed on successfull parse
            EdiItems = ediItems,
        };
    }

    public ParseResult ParseFile(string filename)
    {
        if (filename is null)
        {
            string err = "Filename missing";
            Console.WriteLine(err);
            return new ParseResult { Success = false, Message = err };
        }

        if (!File.Exists(filename))
        {
            string err = $"File not found: {filename}";
            Console.WriteLine(err);
            return new ParseResult { Success = false, Message = err };
        }

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
    
    /*
    public ParseResult ParseFile(string filename)
    {
		if (filename == null)
		{
            string err = "Filename missing";
            Console.WriteLine(err);
			return new ParseResult { Success = false, Message = err};
		}

		if (!System.IO.File.Exists(filename))
        {
            string err = $"File not found: {filename}";
            Console.WriteLine();
            return new ParseResult { Success = false, Message = err };
        }

        string hash = CreateHash(filename);

        License.SetSerial(this.licenseKey);

        List<IEdiItem> ediItems;

        X12ReaderSettings x12ReaderSettings = new()
        {
            ContinueOnError = true,
        };

        using (Stream ediStream = File.OpenRead(filename))
        {
            using (var ediReader = new X12Reader(ediStream, "EdiFabric.Templates.Hipaa", x12ReaderSettings))
                ediItems = ediReader.ReadToEnd().Where(x => x is not null).ToList();
        }

        if(ediItems is null)
        {
            return new()
            {
                Success = false,
                Message = "Parse returned null EdiItems",
                SourceFileName = filename,
                SourceHash = hash
            };
        }


              
        var errors = ediItems.OfType<ReaderErrorContext>();
        if (errors.Any())
        {
            StringBuilder builder = new();
            Console.WriteLine($"Errors detected while parsing file.  Count of errors: {errors.Count()}");
            foreach(var err in errors.Where(x => x is not null))
            {
                if(builder.Length > 0)
                    builder.Append('\n');

                builder.Append(err.Message);
                
            }

            builder.Insert(0, "Parse errors detected: ");

            string errorSummary = builder.ToString();

            return new()
            {
                Success = false,
                Message = errorSummary,
                SourceFileName = filename,
                SourceHash = hash,
                EdiItems = ediItems
            };
        }
       
       
        

        return new()
        {
            Success = true,
            Message = String.Empty, // no message needed on successfull parse
            EdiItems = ediItems,
            SourceFileName = filename,
            SourceHash = hash
        };
    }

    */

    private static string CreateHash(string filename)
    {
        using Stream stream = File.OpenRead(filename);
        byte[] hash = SHA256.HashData(stream);
        return Convert.ToHexString(hash);
    }

}


public interface IEdiParser
{
    ParseResult ParseFile(string filename);
}