using ContractingService.Application.DTOs;
using ContractingService.Application.Interfaces;
using ContractingService.Domain.Entities;
using ContractingService.Domain.Interfaces;

namespace ContractingService.Application.Services;

public class ContractService : IContractService
{
    private readonly IContractRepository _repository;

    public ContractService(IContractRepository repository)
    {
        _repository = repository;
    }

    public async Task CreateContractFromProposalAsync(ProposalApprovedDto dto)
    {

        var contract = new Contract(dto.Id, dto.CustomerName, dto.InsuredAmount);

        await _repository.AddAsync(contract);
    }

    public async Task<ContractResponseDto?> GetContractByIdAsync(Guid id)
    {
        var contract = await _repository.GetByIdAsync(id);

        if (contract is null) return null;

        return new ContractResponseDto(
            contract.Id,
            contract.CustomerName,
            contract.InsuredAmount,
            contract.ContractedAt.HasValue, 
            contract.ContractedAt,
            contract.CreatedAt
        );
    }

    public async Task SignContractAsync(Guid id, SignContractDto dto)
    {
        var contract = await _repository.GetByIdAsync(id);
        
        if (contract is null)
            throw new KeyNotFoundException($"Contrato {id} não encontrado.");

        contract.SignContract(dto.ContractedAt);

        await _repository.UpdateAsync(contract);
    }
    public async Task<IEnumerable<ContractResponseDto>> GetAllContractsAsync()
    {
        var contracts = await _repository.GetAllAsync();

        return contracts.Select(c => new ContractResponseDto(
            c.Id,
            c.CustomerName,
            c.InsuredAmount,
            c.ContractedAt.HasValue,
            c.ContractedAt,
            c.CreatedAt
        ));
    }
}