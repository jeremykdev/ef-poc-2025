using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.ErrorContexts;
using EdiFabric.Core.Model.Edi.X12;
using EdiIngestConsole.Models;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace EdiIngestConsole.Validations;

/// <summary>
/// Performs validation
/// </summary>
/// <remarks>
/// Depends on SNIP level in enviroment variable "SNIP_VALIDATION_LEVEL"
/// Value should be an integer that maps to a ValidationLevel enum value
/// see: https://www.edifabric.com/api/html/T_EdiFabric_Core_Model_Edi_ValidationLevel.htm
/// </remarks>
public class Validator : IValidator
{
    protected const ValidationLevel DefaultValidationLevel = ValidationLevel.SyntaxOnly_SNIP1;

    public ValidationResult Validate(ControlSegment controlSegment)
    {
        ArgumentNullException.ThrowIfNull(controlSegment);

        ValidationLevel validationLevel = this.GetValidationLevel();

        ValidationSettings settings = new()
        {
            ValidationLevel = validationLevel,
        };

        // if input is IEA convert to custom class to apply custom validation
        if (controlSegment is IEA)
            controlSegment = new IeaCustom((controlSegment as IEA)!);

        // list will be empty if/when segment is valid 
        List<SegmentErrorContext> errors = controlSegment.Validate(settings);

        if (errors is null || errors.Count == 0)
            return new();

        MessageErrorContext messageError = new();
        messageError.AddRange(errors.Where(x => x is not null));
        
        return new(messageError);
    }

    public ValidationResult Validate(EdiMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        // for testing SNIP level 5 validation will extend the valid code values
        // for BHT  hierarchical structure code which currently supports only '0019'
        // for 837P
        // see: https://www.edifabric.com/docs/edifabric-net/validate-edi-with-templates.html#what-is-dataelementcodesmap
        
        var codeSetMap = new Dictionary<string, List<string>>
        {
            { "X12_ID_1005_5", new List<string> { "0019", "TEST" } }
        };

        ValidationLevel validationLevel = this.GetValidationLevel();

        ValidationSettings settings = new()
        {
            ValidationLevel = validationLevel,
            DataElementCodesMap = codeSetMap
        };

        MessageErrorContext? errorContext = null;

        bool isValid = message.IsValid(out errorContext, settings);

        if (isValid)
              return new();
       
        return new(errorContext);
    }


    protected ValidationLevel GetValidationLevel()
    {
        // look for validation level in enviroment

        string? envValidationLevel = Environment.GetEnvironmentVariable("SNIP_VALIDATION_LEVEL");

        if (Int32.TryParse(envValidationLevel, out int result))
        {
            if (Enum.IsDefined<ValidationLevel>((ValidationLevel)result))
            {
                var foundLevel = (ValidationLevel)result;
                Console.WriteLine($"Using validation level: {foundLevel}");
                return foundLevel;
            }
        }

        Console.WriteLine($"No valid validation level found.  Using default: {DefaultValidationLevel}");
        return DefaultValidationLevel;
    }

}


public interface IValidator
{
    ValidationResult Validate(EdiMessage message);

    ValidationResult Validate(ControlSegment controlSegment);
}