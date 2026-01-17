using Dapper;
using System.Data;
using ContractingService.Domain.Entities;
using ContractingService.Domain.Interfaces;

namespace ContractingService.Infrastructure.Repositories;

public class ContractingRepository : IContractRepository
{
    private readonly IDbConnection _dbConnection;

    public ContractingRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task AddAsync(Contract contract)
    {
        var sql = @"
            INSERT INTO contracting (
                id, 
                customerName, 
                insuredAmount, 
                created_at, 
                modified_at,
                contracted_at
            )
            VALUES (
                @Id, 
                @CustomerName, 
                @InsuredAmount, 
                @CreatedAt, 
                @UpdatedAt,
                @ContractedAt
            )";

        await _dbConnection.ExecuteAsync(sql, contract);
    }

    public async Task<IEnumerable<Contract>> GetAllAsync()
    {
        var sql = @"
            SELECT 
                id as Id, 
                customerName as CustomerName, 
                insuredAmount as InsuredAmount, 
                created_at as CreatedAt, 
                modified_at as UpdatedAt,
                contracted_at as ContractedAt
            FROM contracting";

        return await _dbConnection.QueryAsync<Contract>(sql);
    }

    public async Task<Contract?> GetByIdAsync(Guid id)
    {
        var sql = @"
            SELECT 
                id as Id, 
                customerName as CustomerName, 
                insuredAmount as InsuredAmount, 
                created_at as CreatedAt, 
                modified_at as UpdatedAt,
                contracted_at as ContractedAt
            FROM contracting 
            WHERE id = @Id";

        return await _dbConnection.QuerySingleOrDefaultAsync<Contract>(sql, new { Id = id });
    }

    public async Task UpdateAsync(Contract contract)
    {
    
        var sql = @"
            UPDATE contracting 
            SET 
                modified_at = @UpdatedAt,
                insuredAmount = @InsuredAmount, 
                customerName = @CustomerName,
                contracted_at = @ContractedAt
            WHERE id = @Id";

        await _dbConnection.ExecuteAsync(sql, contract);
    }

    public async Task DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM contracting WHERE id = @Id";
        await _dbConnection.ExecuteAsync(sql, new { Id = id });
    }
}