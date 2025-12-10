using TicketMonster.Domain.Enums;

namespace TicketMonster.DTOs;

public record PaymentResponse(
    Guid Id,
    decimal Value,
    DateTime CreationDate,
    DateTime ExpirationDate,
    PaymentStatus Status,
    string? ClientSecret,
    Guid TicketId
);
