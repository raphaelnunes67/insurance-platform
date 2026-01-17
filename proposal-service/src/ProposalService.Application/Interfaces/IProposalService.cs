using ProposalService.Application.DTOs;

namespace ProposalService.Application.Interfaces;

public interface IProposalService
{
    Task<Guid> CreateContractAsync(CreateProposalDto dto);
    Task DeleteContractAsync(Guid id);
    Task UpdateContractStatusAsync(Guid id, UpdateStatusDto dto);
    Task<ProposalResponseDto?> GetProposalByIdAsync(Guid id);
    Task UpdateProposalAsync(Guid id, UpdateProposalDto dto);
}