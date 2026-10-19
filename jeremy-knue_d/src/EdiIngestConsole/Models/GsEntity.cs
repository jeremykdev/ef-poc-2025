using EdiFabric.Core.Model.Edi.X12;
using System.ComponentModel.DataAnnotations.Schema;

namespace EdiIngestConsole.Models;

[Table("Gs")]
public class GsEntity : GS
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } // will be used for database id


    public GsEntity()
    {
        
    }

    public GsEntity(GS source)
    {
        this.CodeIdentifyingInformationType_1 = source.CodeIdentifyingInformationType_1;
        this.Date_4 = source.Date_4;
        this.GroupControlNumber_6 = source.GroupControlNumber_6;
        this.ReceiverIDCode_3 = source.ReceiverIDCode_3;
        this.SenderIDCode_2 = source.SenderIDCode_2;
        this.Time_5 = source.Time_5;
        this.TransactionTypeCode_7 = source.TransactionTypeCode_7;
        this.VersionAndRelease_8 = source.VersionAndRelease_8;
    }

}
