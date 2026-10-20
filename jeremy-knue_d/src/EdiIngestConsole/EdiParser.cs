using EdiFabric;
using EdiFabric.Core.Model.Edi;
using EdiFabric.Framework.Readers;

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

    public List<IEdiItem> ParseFile(string filename)
    {
		if (filename == null)
		{
			Console.WriteLine("File name missing");
			return [];
		}

		if (!System.IO.File.Exists(filename))
        {
            Console.WriteLine($"File not found: {filename}");
            return [];
        }

        License.SetSerial(this.licenseKey);

        using Stream ediStream = File.OpenRead(filename);

        List<IEdiItem> ediItems;
        using (var ediReader = new X12Reader(ediStream, "EdiFabric.Templates.Hipaa"))
            ediItems = ediReader.ReadToEnd().ToList();

        return ediItems;
    }
}


public interface IEdiParser
{
    List<IEdiItem> ParseFile(string filename);
}