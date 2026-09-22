using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.DTO.Location;
using Warehouse.Application.Interfaces;
using FluentValidation;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;
    private readonly ILogger<LocationsController> _logger;
    private readonly IValidator<LocationDto> _validator;
    
    public LocationsController(
        ILocationService locationService,
        ILogger<LocationsController> logger,
        IValidator<LocationDto> validator)
    {
        _locationService = locationService;
        _logger = logger;
        _validator = validator;
    }

    // GET: api/locations
    [HttpGet]
    [EndpointSummary("Location list")]
    public async Task<ActionResult<IReadOnlyList<LocationResponseDto>>> GetLocations()
    {
        var locations = await _locationService.GetAllAsync();
        return Ok(locations);
    }

    // GET: api/locations/{id}
    [HttpGet("{id:int}")]
    [EndpointSummary("Returns location by id")]
    [ProducesResponseType(typeof(LocationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LocationResponseDto>> GetLocationById(int id)
    {
        var location = await _locationService.GetByIdAsync(id);

        if (location is null)
            return NotFound();

        return Ok(location);
    }

    // POST: api/locations/add
    [HttpPost("add")]
    [EndpointSummary("Inserts location into database")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LocationResponseDto>> AddLocation(LocationDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }
        
        _logger.LogDebug(
            "Adding location: {Name}, Code: {Code}, Warehouse: {WarehouseCode}",
            dto.Name,
            dto.Code,
            dto.WarehouseCode);

        var result = await _locationService.AddAsync(dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        var location = result.Location!;

        return CreatedAtAction(
            nameof(GetLocationById),
            new { id = location.Id },
            location);
    }

    // PUT: api/locations/update/{id}
    [HttpPut("update/{id:int}")]
    [EndpointSummary("Updates location")]
    [ProducesResponseType(typeof(LocationResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LocationResponseDto>> UpdateLocation(int id, LocationDto dto)
    {
        var validationResult = await _validator.ValidateAsync(dto);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }
        
        var result = await _locationService.UpdateAsync(id, dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        return Ok(result.Location);
    }
    
    // DELETE: api/locations/delete/{id}
    [HttpDelete("delete/{id:int}")]
    [EndpointSummary("Deletes location from database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var deleted = await _locationService.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}