using EdiFabric.Core.Model.Edi.ErrorContexts;
using System.Text;

namespace EdiIngestConsole.Validations;

public class ValidationResult
{
    public ValidationResult()
    {
        
    }

    
    public ValidationResult(MessageErrorContext? errorContext)
    {
        if (errorContext is not null)
            this.ErrorMessages = errorContext!.Flatten()?.Where(x => !String.IsNullOrWhiteSpace(x))?.ToArray() ?? [];
    }


    public bool IsValid() => !this.ErrorMessages.Any();


    public string[] ErrorMessages = [];

    public override string ToString()
    {
        return this.ToString(';');
    }

    public string ToString(char delimiter)
    {
        StringBuilder builder = new();
        builder.Append($"Is valid: {this.IsValid()}");

        foreach(var item in this.ErrorMessages)
        {
            builder.Append(delimiter);
            builder.Append(item);
        }

        return builder.ToString();
    }
}
