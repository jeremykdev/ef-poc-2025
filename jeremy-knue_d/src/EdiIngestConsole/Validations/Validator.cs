using EdiFabric.Core.Model.Edi;
using EdiFabric.Core.Model.Edi.ErrorContexts;

namespace EdiIngestConsole.Validations;

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
        };

        MessageErrorContext? errorContext = null;

        bool isValid = message.IsValid(out errorContext);

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