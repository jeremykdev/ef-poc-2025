namespace EdiIngestConsole.Parsers;

public class LocalFileProvider : IFileProvider
{
    private string _fileName;

    public LocalFileProvider(string fileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);

        if (!File.Exists(fileName))
            throw new FileNotFoundException(fileName);

        this._fileName = fileName;        
    }

    public string GetFileName() =>
            Path.GetFileName(this._fileName);
    

    public Stream GetStream(bool useAysnc) =>    
        new FileStream(this._fileName, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAysnc);
    
}
