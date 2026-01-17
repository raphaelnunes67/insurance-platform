using Microsoft.AspNetCore.Mvc;
using ContractingService.Api.Contracts.Contracts;
using ContractingService.Application.DTOs;        
using ContractingService.Application.Interfaces;  

namespace ContractingService.Api.Controllers;

[ApiController]
[Route("api/contracts")]
public class ContractsController : ControllerBase
{
    private readonly IContractService _service;

    public ContractsController(IContractService service)
    {
        _service = service;
    }
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ContractResponse>> GetById(Guid id)
    {
        var contractDto = await _service.GetContractByIdAsync(id);

        if (contractDto is null)
            return NotFound();

        var response = new ContractResponse(
            contractDto.Id,
            contractDto.CustomerName,
            contractDto.InsuredAmount,
            contractDto.IsSigned,
            contractDto.ContractedAt,
            contractDto.CreatedAt
        );

        return Ok(response);
    }

    [HttpPatch("{id:guid}/signed-date")]
    public async Task<IActionResult> SignContract(
        Guid id, 
        [FromBody] SignContractRequest request)
    {
        try
        {
            var dto = new SignContractDto(request.ContractedAt);

            await _service.SignContractAsync(id, dto);

            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ContractResponse>>> List()
    {
        var contracts = await _service.GetAllContractsAsync();

        var response = contracts.Select(c => new ContractResponse(
            c.Id,
            c.CustomerName,
            c.InsuredAmount,
            c.IsSigned,
            c.ContractedAt,
            c.CreatedAt
        ));

        return Ok(response);
    }
}