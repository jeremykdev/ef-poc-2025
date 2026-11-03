using EdiFabric.Core.Annotations.Edi;
using EdiFabric.Core.Annotations.Validation;
using EdiFabric.Templates.Hipaa5010;
using System.Runtime.Serialization;

namespace EdiIngestConsole.Models;

public class BHT_BeginningOfHierarchicalTransaction_8Custom : BHT_BeginningOfHierarchicalTransaction_8
{
    [DataMember]
    [Pos(7)]
    [Required]
    public string ExtraPayload { get; set; }
}
