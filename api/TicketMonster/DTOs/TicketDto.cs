namespace TicketMonster.DTOs;

public record CreateTicketRequest(
    Guid ShowId,
    Guid PaymentId
);

public record UpdateTicketRequest(
    Guid ShowId,
    Guid PaymentId
);

public record TicketResponse(
    Guid Id,
    Guid Code,
    string UserOwnerId,
    Guid ShowId,
    Guid PaymentId
);
