using CW.Core.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace CW.Api;

internal static class ValidationHelper
{
    internal static bool Validate<TRequest>(TRequest updateOrderRequest, out List<ValidationResult> validationErrors)
    {
        var errors = new List<ValidationResult>();

        if (updateOrderRequest is null)
        {
            validationErrors = [new("Request is null")];
            return false;
        }

        if (Validator.TryValidateObject(updateOrderRequest, new ValidationContext(updateOrderRequest), errors, true))
        {
            validationErrors = [];
            return true;
        }

        validationErrors = errors;
        return false;
    }

    internal static void ThrowValidationException(List<ValidationResult> validationErrors)
    {
        throw new InputValidationException(validationErrors);
    }
}