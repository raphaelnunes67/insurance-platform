namespace ContractingService.Api.Contracts.Contracts;

public record ContractResponse(
    Guid Id,
    string CustomerName,
    decimal InsuredAmount,
    bool IsSigned,
    DateTime? ContractedAt,
    DateTime CreatedAt
);