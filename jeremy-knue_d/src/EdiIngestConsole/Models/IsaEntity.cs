using System;
using System.ComponentModel.DataAnnotations.Schema;
using EdiFabric.Core.Model.Edi.X12;
using Microsoft.EntityFrameworkCore;

namespace EdiIngestConsole.Models;

[Table("Isa")]
public class IsaEntity : ISA
{

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } // will be used for database id

    public IsaEntity()
    {
        // empty ctor to support serialization
    }

    public IsaEntity(ISA source)
    {
        this.AcknowledgementRequested_14 = source.AcknowledgementRequested_14;
        this.AuthorizationInformation_2 = source.AuthorizationInformation_2;
        this.AuthorizationInformationQualifier_1 = source.AuthorizationInformationQualifier_1;
        this.ComponentElementSeparator_16 = source.ComponentElementSeparator_16;
        this.InterchangeControlNumber_13 = source.InterchangeControlNumber_13;
        this.InterchangeControlStandardsIdentifier_11 = source.InterchangeControlStandardsIdentifier_11;
        this.InterchangeControlVersionNumber_12 = source.InterchangeControlVersionNumber_12;
        this.InterchangeDate_9 = source.InterchangeDate_9;
        this.InterchangeReceiverID_8 = source.InterchangeReceiverID_8;
        this.InterchangeSenderID_6 = source.InterchangeSenderID_6;
        this.InterchangeTime_10 = source.InterchangeTime_10;
        this.ReceiverIDQualifier_7 = source.ReceiverIDQualifier_7;
        this.SecurityInformation_4 = source.SecurityInformation_4;
        this.SecurityInformationQualifier_3 = source.SecurityInformationQualifier_3;
        this.SenderIDQualifier_5 = source.SenderIDQualifier_5;
        this.UsageIndicator_15 = source.UsageIndicator_15;
    }
}
