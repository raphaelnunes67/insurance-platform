using ContractingService.Application.DTOs;

namespace ContractingService.Application.Interfaces;

public interface IContractService
{
    Task CreateContractFromProposalAsync(ProposalApprovedDto dto);

    Task<ContractResponseDto?> GetContractByIdAsync(Guid id);

    Task SignContractAsync(Guid id, SignContractDto dto);

    Task<IEnumerable<ContractResponseDto>> GetAllContractsAsync();
}