using System.ComponentModel.DataAnnotations;

namespace CW.Api.Models;

public record UpdateOrderRequest()
{
    [Required]
    public required Guid Id { get; init; }

    [Length(3, 36)]
    public required string Text { get; init; }

    [Range(1, 10)]
    public required int Count { get; init; }

    [Range(1, 100_000)]
    public required decimal TotalAmount { get; init; }
}
