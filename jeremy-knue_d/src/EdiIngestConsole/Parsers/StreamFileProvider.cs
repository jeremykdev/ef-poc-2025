namespace EdiIngestConsole.Parsers;

public class StreamFileProvider : IFileProvider 
{
    private Stream _stream;

    private string _fileName;

    public StreamFileProvider(Stream stream, string filename)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentException.ThrowIfNullOrWhiteSpace(filename);

        this._stream = stream;
        this._fileName = filename;
    }

    

    public string GetFileName() => this._fileName;

    public Stream GetStream(bool useAsync) => this._stream;
}
