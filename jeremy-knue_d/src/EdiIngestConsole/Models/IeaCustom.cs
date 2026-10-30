using EdiFabric.Core.Annotations.Validation;
using EdiFabric.Core.ErrorCodes;
using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.ErrorContexts;
using EdiFabric.Core.Model.Edi.X12;

namespace EdiIngestConsole.Models;

/// <summary>
/// Custom IEA implementation with validation
/// </summary>
/// <remarks>Demostrates SNIP level 6 validation</remarks>
public class IeaCustom : IEA, IEdiValidator
{


    protected const string InterchangeControlNumberMustEndWith = "101";

    public IeaCustom()
    {
        
    }

    public IeaCustom(IEA baseIea)
    {
        this.NumberOfIncludedGroups_1 = baseIea.NumberOfIncludedGroups_1;
        this.InterchangeControlNumber_2 = baseIea.InterchangeControlNumber_2;
    }

    public SegmentErrorContext ValidateEdi(ValidationContext validationContext)
    {
        SegmentErrorContext? resultErrorContext = null;

        // our custom validation rule is that interchange control number must end with string specified in constant 
        if (this.InterchangeControlNumber_2 is null || !this.InterchangeControlNumber_2.EndsWith(InterchangeControlNumberMustEndWith))
        {
            resultErrorContext = new();

            DataElementErrorContext dataElemErr = new(
                name: "InterchangeControlNumber",
                position: validationContext.InSegmentIndex,
                code: DataElementErrorCode.InvalidCodeValue,
                componentPosition: validationContext.InCompositeIndex,
                repetitionPosition: validationContext.RepetitionIndex,
                value: this.InterchangeControlNumber_2 ?? "{null}",
                message: $"Custom validtion - InterchangeControlNumber must end with {InterchangeControlNumberMustEndWith}"
                );

            resultErrorContext.Add(dataElemErr);
        }

        return resultErrorContext;
    }
}
