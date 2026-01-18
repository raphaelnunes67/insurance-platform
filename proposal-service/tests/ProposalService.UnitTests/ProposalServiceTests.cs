using Xunit;
using Moq;
using ProposalService.Application.DTOs;
using ProposalService.Application.Interfaces;
using ProposalService.Domain.Entities;
using ProposalService.Domain.Interfaces;

// Alias to avoid conflict "ProposalService" and "ProposalService" class
using ServiceImplementation = ProposalService.Application.Services.ProposalService;

namespace ProposalService.UnitTests;

public class ProposalServiceTests
{
    private readonly Mock<IProposalRepository> _repositoryMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly ServiceImplementation _service;

    public ProposalServiceTests()
    {
        _repositoryMock = new Mock<IProposalRepository>();
        _eventBusMock = new Mock<IEventBus>();
        _service = new ServiceImplementation(_repositoryMock.Object, _eventBusMock.Object);
    }

    [Fact]
    public async Task CreateContract_Should_CallRepositoryAdd_When_Valid()
    {
        var createDto = new CreateProposalDto("Cliente Teste", 10000m);

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<Proposal>()))
                       .Returns(Task.CompletedTask);

        var resultId = await _service.CreateContractAsync(createDto);

        Assert.NotEqual(Guid.Empty, resultId);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<Proposal>()), Times.Once);
    }

    [Fact]
    public async Task UpdateContractStatus_Should_Approve_And_PublishEvent()
    {
        var proposalId = Guid.NewGuid();
        
        var updateDto = new UpdateStatusDto("APPROVED");
        
        var existingProposal = new Proposal("Cliente Feliz", 5000m);
        
        _repositoryMock.Setup(r => r.GetByIdAsync(proposalId))
                       .ReturnsAsync(existingProposal);

        await _service.UpdateContractStatusAsync(proposalId, updateDto);

        _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Proposal>()), Times.Once);
        _eventBusMock.Verify(bus => bus.PublishProposalApprovedAsync(It.IsAny<Proposal>()), Times.Once);
    }

    [Fact]
    public async Task UpdateContractStatus_Should_ThrowException_When_NotFound()
    {
        var proposalId = Guid.NewGuid();
        
        var updateDto = new UpdateStatusDto("APPROVED");

        _repositoryMock.Setup(r => r.GetByIdAsync(proposalId))
                       .ReturnsAsync((Proposal?)null);

        var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => 
            _service.UpdateContractStatusAsync(proposalId, updateDto));

        Assert.Equal($"Proposta {proposalId} não encontrada.", exception.Message);
        _eventBusMock.Verify(bus => bus.PublishProposalApprovedAsync(It.IsAny<Proposal>()), Times.Never);
    }
}