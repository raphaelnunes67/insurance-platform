using Dapper;
using System.Data;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Interfaces;

namespace ProposalService.Infrastructure.Repositories;

public class ProposalRepository : IProposalRepository
{
    private readonly IDbConnection _dbConnection;

    public ProposalRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task AddAsync(Proposal proposal)
    {
        var sql = @"
            INSERT INTO Proposals (
                Id, 
                CustomerName, 
                IsuredAmount, 
                Status, 
                CreatedAt, 
                UpdatedAt
            )
            VALUES (
                @Id, 
                @CustomerName, 
                @InsuredAmount, 
                @Status, 
                @CreatedAt, 
                @UpdatedAt
            )";

        await _dbConnection.ExecuteAsync(sql, proposal);
    }

    public async Task<IEnumerable<Proposal>> GetAllAsync()
{
    var sql = @"
        SELECT 
            Id, 
            CustomerName, 
            IsuredAmount as InsuredAmount, 
            Status, 
            CreatedAt, 
            UpdatedAt 
        FROM Proposals";

    return await _dbConnection.QueryAsync<Proposal>(sql);
}
    public async Task<Proposal?> GetByIdAsync(Guid id)
    {
        // Mapeamos IsuredAmount (banco) para InsuredAmount (C#)
        var sql = @"
        SELECT 
            Id, 
            CustomerName, 
            IsuredAmount as InsuredAmount, 
            Status, 
            CreatedAt, 
            UpdatedAt 
        FROM Proposals 
        WHERE Id = @Id";

        return await _dbConnection.QuerySingleOrDefaultAsync<Proposal>(sql, new { Id = id });
    }

    public async Task UpdateAsync(Proposal proposal)
    {
        var sql = @"
        UPDATE Proposals 
        SET 
            Status = @Status,
            UpdatedAt = @UpdatedAt,
            IsuredAmount = @InsuredAmount, 
            CustomerName = @CustomerName
        WHERE Id = @Id";

        await _dbConnection.ExecuteAsync(sql, proposal);
    }

    public async Task DeleteAsync(Guid id)
    {
        var sql = "DELETE FROM Proposals WHERE Id = @Id";
        await _dbConnection.ExecuteAsync(sql, new { Id = id });
    }
}