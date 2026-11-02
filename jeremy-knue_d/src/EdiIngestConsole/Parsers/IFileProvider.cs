namespace EdiIngestConsole.Parsers;

public interface IFileProvider
{
    public Stream GetStream(bool useAsync); 

    public string GetFileName();
}
