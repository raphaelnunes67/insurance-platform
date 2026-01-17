namespace ContractingService.Application.DTOs;

public record ContractResponseDto(
    Guid Id,
    string CustomerName,
    decimal InsuredAmount,
    bool IsSigned,           
    DateTime? ContractedAt,
    DateTime CreatedAt
);