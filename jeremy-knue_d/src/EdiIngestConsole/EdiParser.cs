using System;
using EdiFabric;
using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.X12;
using EdiFabric.Templates.Hipaa5010;
using EdiFabric.Framework;
using EdiFabric.Framework.Readers;
using EdiFabric.Framework.Writers;
using System.Text;

namespace EdiIngestConsole;

public class EdiParser
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
