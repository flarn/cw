using CW.Core.interfaces;

namespace CW.Core.Events;

public record OrderUpdatedEvent(
    Guid Id,
    string Text,
    int Count,
    decimal TotalAmount,
    DateTimeOffset UpdatedAt
    ) : IEvent;
