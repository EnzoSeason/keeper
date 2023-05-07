using System.ComponentModel.DataAnnotations;

namespace Reporting.Domain.SeedWork;

public static class DomainModelValidator<T> where T : IValidatableObject
{
    public static bool TryValidate(T model) =>
        Validator.TryValidateObject(model, new ValidationContext(model, null, null), null, true);

    public static bool TryValidate(T model, out ICollection<ValidationResult> validationResults)
    {
        var results = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(model, new ValidationContext(model, null, null), results, true);

        validationResults = results;
        return isValid;
    }
}