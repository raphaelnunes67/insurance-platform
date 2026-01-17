namespace ProposalService.Api.Contracts.Proposals;

public record CreateProposalRequest(string CustomerName, decimal InsuredAmount);