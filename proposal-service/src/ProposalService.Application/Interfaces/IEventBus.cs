using ProposalService.Domain.Entities;

namespace ProposalService.Application.Interfaces;

public interface IEventBus
{
    Task PublishProposalApprovedAsync(Proposal proposal);
}