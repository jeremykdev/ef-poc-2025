using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.ErrorContexts;
using EdiFabric.Core.Model.Edi.X12;

namespace EdiIngestConsole.Validations;

/// <summary>
/// Performs SNIP and Syntax validation
/// </summary>
/// <remarks>
/// Depends on SNIP level in enviroment variable "SNIP_VALIDATION_LEVEL"
/// Value should be an integer that maps to a ValidationLevel enum value
/// see: https://www.edifabric.com/api/html/T_EdiFabric_Core_Model_Edi_ValidationLevel.htm
/// Checks extended syntax
/// See: https://www.edifabric.com/docs/edifabric-net/validate-edi-with-templates.html#how-to-validate-x12-data-types
/// </remarks>
public class Validator : IValidator
{
    protected const ValidationLevel DefaultValidationLevel = ValidationLevel.SyntaxOnly_SNIP1;


    public ValidationResult Validate(EdiMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        ValidationLevel validationLevel = this.GetValidationLevel();

        ValidationSettings settings = new()
        {
            ValidationLevel = validationLevel,
            SyntaxSet = new Extended(),
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
}