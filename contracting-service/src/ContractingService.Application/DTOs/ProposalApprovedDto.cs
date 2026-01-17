namespace ContractingService.Application.DTOs;

public record ProposalApprovedDto(
    Guid Id, 
    string CustomerName, 
    decimal InsuredAmount,
    DateTime ApprovedAt
);