using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdiIngestConsole.Models;

/// <summary>
/// Entity for persisting history of parsed files
/// </summary>
[Table("History")]
public class History
{
    public History()
    {
        
    }

    public History(ParseResult parseResult)
    {
        ArgumentNullException.ThrowIfNull(parseResult);

        this.SourceFilename = parseResult.SourceFileName ?? String.Empty;
        this.SourceHash = parseResult.SourceHash ?? String.Empty;
        this.Success = parseResult.Success;
        this.Message = parseResult.Message ?? String.Empty;
    }

     
    public int Id { get; set; } // by default EF should treat this as an identity

    [Required]
    public string SourceFilename { get; set; } = String.Empty;

    [Required]
    public string SourceHash { get; set; } = String.Empty;

    public DateTimeOffset DateProcessed { get; set; } = DateTimeOffset.UtcNow;

    public bool Success { get; set; }

    [Required]
    public string Message { get; set; } = String.Empty;

}
