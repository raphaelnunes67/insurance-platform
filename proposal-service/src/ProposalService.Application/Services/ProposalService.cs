using ProposalService.Application.DTOs;
using ProposalService.Application.Interfaces;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Interfaces;

namespace ProposalService.Application.Services;

public class ProposalService : IProposalService
{
    private readonly IProposalRepository _proposalRepository;
    private readonly IEventBus _eventBus;

    public ProposalService(IProposalRepository proposalRepository, IEventBus eventBus)
    {
        _proposalRepository = proposalRepository;
        _eventBus = eventBus;
    }

    public async Task<Guid> CreateContractAsync(CreateProposalDto dto)
    {
        var proposal = new Proposal(dto.CustomerName, dto.InsuredAmount);

        await _proposalRepository.AddAsync(proposal);

        return proposal.Id;
    }
    public async Task UpdateContractStatusAsync(Guid id, UpdateStatusDto dto)
    {
        var proposal = await _proposalRepository.GetByIdAsync(id);
        
        if (proposal is null)
            throw new KeyNotFoundException($"Proposta {id} não encontrada.");

        var status = dto.NewStatus.ToUpper();
        bool isApproved = false;
        if (status == "APPROVED")
        {
            proposal.Approve();
            isApproved = true;
        }
        else if (status == "REJECTED")
        {
            proposal.Reject();
        }

        await _proposalRepository.UpdateAsync(proposal);

        if (isApproved)
        {
            await _eventBus.PublishProposalApprovedAsync(proposal);
        }
    }

    public async Task DeleteContractAsync(Guid id)
    {
        var proposal = await _proposalRepository.GetByIdAsync(id);
        if (proposal is null) return; 

        await _proposalRepository.DeleteAsync(id);
    }

    public async Task<ProposalResponseDto?> GetProposalByIdAsync(Guid id)
    {
        var entity = await _proposalRepository.GetByIdAsync(id);

        if (entity is null) return null;
        return new ProposalResponseDto(
            entity.Id,
            entity.CustomerName,
            entity.InsuredAmount,
            entity.Status,
            entity.CreatedAt,
            entity.UpdatedAt
        );
    }

    public async Task UpdateProposalAsync(Guid id, UpdateProposalDto dto)
    {
        var proposal = await _proposalRepository.GetByIdAsync(id);

        if (proposal is null)
            throw new KeyNotFoundException($"Proposta {id} não encontrada.");

        proposal.UpdateInformation(dto.CustomerName, dto.InsuredAmount);

        await _proposalRepository.UpdateAsync(proposal);
    }
}