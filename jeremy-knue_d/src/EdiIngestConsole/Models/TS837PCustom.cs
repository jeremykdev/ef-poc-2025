using EdiFabric.Core.Annotations.Edi;
using EdiFabric.Core.Annotations.Validation;
using EdiFabric.Templates.Hipaa5010;
using System.Runtime.Serialization;

namespace EdiIngestConsole.Models;

public class TS837PCustom : TS837P
{
    [DataMember]
    [Pos(2)]
    [Required]
    public BHT_BeginningOfHierarchicalTransaction_8Custom BHT_BeginningOfHierarchicalTransaction { get; set; }
}
