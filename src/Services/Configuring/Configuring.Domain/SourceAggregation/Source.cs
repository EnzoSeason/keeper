using System.ComponentModel.DataAnnotations;
using Domain.SeedWork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Configuring.Domain.SourceAggregation;

public record Source: IAggregateRoot, IValidatableObject
{
    public ObjectId Id { get; set; }
    
    [BsonRepresentation(BsonType.String)]
    public Guid ConfigId { get; init; }

    /// <summary>
    /// Source name. It's descriptive.
    /// <para>e.g. Jack's bank accounts</para>
    /// </summary>
    public string Name { get; init; } = null!;
    
    public IEnumerable<Category> Categories { get; init; } = null!;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (string.IsNullOrEmpty(Name))
        {
            results.Add(new ValidationResult("Name is required.", new[] { nameof(Name) }));
        }

        if (!Categories.Any())
        {
            results.Add(new ValidationResult("Categories are required.", new[] { nameof(Categories) }));
        }

        return results;
    }
}