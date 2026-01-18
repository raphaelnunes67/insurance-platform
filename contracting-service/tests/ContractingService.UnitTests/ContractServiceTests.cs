using Xunit;
using Moq;
using ContractingService.Application.Services;
using ContractingService.Application.DTOs;
using ContractingService.Domain.Interfaces;
using ContractingService.Domain.Entities;

namespace ContractingService.UnitTests;

public class ContractServiceTests
{
    private readonly Mock<IContractRepository> _contractRepositoryMock;
    private readonly ContractService _contractService;

    public ContractServiceTests()
    {
        _contractRepositoryMock = new Mock<IContractRepository>();
        _contractService = new ContractService(_contractRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateContractFromProposal_Should_CallRepositoryAdd_When_Valid()
    {
        // Arrange
        // CORREÇÃO: Usando o construtor (Guid, string, decimal, DateTime) exigido pelo erro
        var proposalDto = new ProposalApprovedDto(
            Guid.NewGuid(), 
            "Empresa Teste Ltda", 
            50000m, 
            DateTime.UtcNow
        );

        _contractRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Contract>()))
                               .Returns(Task.CompletedTask);

        // Act
        await _contractService.CreateContractFromProposalAsync(proposalDto);

        // Assert
        _contractRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Contract>()), Times.Once);
    }

    [Fact]
    public async Task SignContract_Should_UpdateRepository_When_ContractExists()
    {
        var contractId = Guid.NewGuid();
        
        var signDto = new SignContractDto(DateTime.UtcNow);

        var existingContract = new Contract(contractId, "Cliente Existente", 1000m); 

        _contractRepositoryMock.Setup(repo => repo.GetByIdAsync(contractId))
                               .ReturnsAsync(existingContract);

        await _contractService.SignContractAsync(contractId, signDto);

        _contractRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Contract>()), Times.Once);
    }

    [Fact]
    public async Task SignContract_Should_ThrowKeyNotFound_When_ContractDoesNotExist()
    {
        var contractId = Guid.NewGuid();
        
        var signDto = new SignContractDto(DateTime.UtcNow);

        _contractRepositoryMock.Setup(repo => repo.GetByIdAsync(contractId))
                               .ReturnsAsync((Contract?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _contractService.SignContractAsync(contractId, signDto));

        Assert.Equal($"Contrato {contractId} não encontrado.", exception.Message);
        _contractRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Contract>()), Times.Never);
    }
}