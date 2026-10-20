using EdiFabric;
using EdiFabric.Core.Model.Edi;
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

        License.SetSerial(this.licenseKey);

        using Stream ediStream = File.OpenRead(filename);

        List<IEdiItem> ediItems;
        using (var ediReader = new X12Reader(ediStream, "EdiFabric.Templates.Hipaa"))
            ediItems = ediReader.ReadToEnd().ToList();        

       
        string hash = CreateHash(filename);

        return new ParseResult
        {
            Success = true,
            Message = "File parsed",
            EdiItems = ediItems,
            SourceFileName = filename,
            SourceHash = hash
        };
    }

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