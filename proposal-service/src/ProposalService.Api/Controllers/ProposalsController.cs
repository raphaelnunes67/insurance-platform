using Microsoft.AspNetCore.Mvc;
using ProposalService.Api.Contracts.Proposals;
using ProposalService.Application.DTOs;     
using ProposalService.Application.Interfaces;

namespace ProposalService.Api.Controllers;

[ApiController]
[Route("api/proposals")]
public sealed class ProposalsController : ControllerBase
{
    private readonly IProposalService _service;

    public ProposalsController(IProposalService service)
    {
        _service = service;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProposalResponseDto>> GetById(Guid id)
    {
        var proposal = await _service.GetProposalByIdAsync(id);

        if (proposal is null)
            return NotFound();

        return Ok(proposal);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProposalDto request)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName) || request.InsuredAmount <= 0)
            return BadRequest("Dados inválidos.");

        try
        {
            await _service.UpdateProposalAsync(id, request);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateProposalRequest request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
            return BadRequest("CustomerName is required.");

        if (request.InsuredAmount <= 0)
            return BadRequest("InsuredAmount must be greater than 0.");

        var createDto = new CreateProposalDto(request.CustomerName, request.InsuredAmount);
        var id = await _service.CreateContractAsync(createDto);
        return CreatedAtAction(nameof(GetById), new { id = id }, new { id = id });
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateStatusDto request)
    {
        try
        {
            await _service.UpdateContractStatusAsync(id, request);
            return NoContent(); 
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex) 
        {
            return Conflict(new { error = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _service.DeleteContractAsync(id);
        return NoContent();
    }
}