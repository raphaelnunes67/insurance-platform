namespace ProposalService.Application.DTOs;

public record ProposalResponseDto(
    Guid Id, 
    string CustomerName, 
    decimal InsuredAmount, 
    string Status, 
    DateTime CreatedAt,
    DateTime UpdatedAt
);