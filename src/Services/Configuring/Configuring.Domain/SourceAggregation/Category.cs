using System.ComponentModel.DataAnnotations;

namespace Configuring.Domain.SourceAggregation;

public record Category: IValidatableObject
{
    public string Name { get; init; } = null!;

    public IEnumerable<string> Keywords { get; init; } = null!;
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();
        
        if (string.IsNullOrEmpty(Name))
        {
            results.Add(new ValidationResult("Name is required.", new[] { nameof(Name) }));
        }

        if (!Keywords.Any())
        {
            results.Add(new ValidationResult("Keywords are required.", new[] { nameof(Keywords) }));
        }

        return results;
    }
}