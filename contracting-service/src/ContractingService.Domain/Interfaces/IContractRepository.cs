using ContractingService.Domain.Entities;

namespace ContractingService.Domain.Interfaces;

public interface IContractRepository
{
    Task AddAsync(Contract contract);
    Task<IEnumerable<Contract>> GetAllAsync();
    Task<Contract?> GetByIdAsync(Guid id);
    Task UpdateAsync(Contract contract);
    Task DeleteAsync(Guid id);
}