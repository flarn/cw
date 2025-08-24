using System.ComponentModel.DataAnnotations;

namespace CW.Core.Exceptions;

public class InputValidationException(List<ValidationResult> validationResults) : CWExceptionBase("Input validation failed")
{
    public List<ValidationResult> ValidationResults { get; } = validationResults;
}
