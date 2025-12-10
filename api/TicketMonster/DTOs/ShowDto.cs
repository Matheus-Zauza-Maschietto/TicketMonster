namespace TicketMonster.DTOs;

public record CreateShowRequest(
    string Title,
    decimal TicketPrice,
    string Singer,
    DateTime PresentationDate,
    long MaxTicketQuantity
);

public record UpdateShowRequest(
    string Title,
    decimal TicketPrice,
    string Singer,
    DateTime PresentationDate,
    long MaxTicketQuantity
);

public record ShowResponse(
    Guid Id,
    string Title,
    decimal TicketPrice,
    string Singer,
    DateTime PresentationDate,
    long MaxTicketQuantity,
    string UserCreatorId
);
