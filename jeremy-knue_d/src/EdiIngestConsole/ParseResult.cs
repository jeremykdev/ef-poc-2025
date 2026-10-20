using EdiFabric.Core.Model.Edi;

namespace EdiIngestConsole;

public class ParseResult
{
    public bool Success { get; set; } 

    public string Message { get; set; } = String.Empty;

    public List<IEdiItem> EdiItems { get; set; } = [];

    public string? SourceFileName { get; set; }

    /// <summary>
    /// Unique hash of file content. 
    /// </summary>
    /// <remarks>Used to detect duplicate</remarks>
    public string? SourceHash { get; set; }
}
