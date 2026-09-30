using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.DTO.ReturnRequest;
using Warehouse.Application.Interfaces;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReturnRequestsController : ControllerBase
{
    private readonly IReturnRequestService _returnRequestService;
    private readonly ILogger<ReturnRequestsController> _logger;

    public ReturnRequestsController(
        IReturnRequestService returnRequestService,
        ILogger<ReturnRequestsController> logger)
    {
        _returnRequestService = returnRequestService;
        _logger = logger;
    }

    // GET: api/returnrequests
    [HttpGet]
    [EndpointSummary("Return request list")]
    [ProducesResponseType(
        typeof(IReadOnlyList<ReturnRequestResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<ReturnRequestResponseDto>>>
        GetReturnRequests()
    {
        var returnRequests =
            await _returnRequestService.GetAllAsync();

        return Ok(returnRequests);
    }

    // GET: api/returnrequests/{id}
    [HttpGet("{id:int}")]
    [EndpointSummary("Returns return request by id")]
    [ProducesResponseType(
        typeof(ReturnRequestResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ReturnRequestResponseDto>>
        GetReturnRequestById(int id)
    {
        var returnRequest =
            await _returnRequestService.GetByIdAsync(id);

        if (returnRequest is null)
            return NotFound();

        return Ok(returnRequest);
    }

    // POST: api/returnrequests
    [HttpPost]
    [EndpointSummary("Creates new return request")]
    [ProducesResponseType(
        typeof(ReturnRequestResponseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ReturnRequestResponseDto>>
        AddReturnRequest(ReturnRequestDto dto)
    {
        var result =
            await _returnRequestService.AddAsync(dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        var returnRequest = result.Data!;

        return CreatedAtAction(
            nameof(GetReturnRequestById),
            new { id = returnRequest.Id },
            returnRequest);
    }
}