using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.DTO;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

[Route("api/[controller]")]
[ApiController]
public class LocationsController : ControllerBase
{
    private readonly WarehouseDbContext _context;
    private readonly ILogger<LocationsController> _logger;

    public LocationsController(
        WarehouseDbContext context,
        ILogger<LocationsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/locations
    [HttpGet]
    [EndpointSummary("Returns all locations")]
    [ProducesResponseType(typeof(IEnumerable<ResultLocationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ResultLocationDto>>> GetLocations()
    {
        var locations = await _context.Locations.Select(x => new ResultLocationDto
        {
            Id = x.Id,
            Code = x.Code,
            WarehouseCode = x.Warehouse.Code,
            Name = x.Name,
            IsActive = x.IsActive
        }).OrderBy(x => x.Id).ToListAsync();

        return Ok(locations);
    }

    // GET: api/locations/5
    [HttpGet("{id}")]
    [EndpointSummary("Returns location by id")]
    [ProducesResponseType(typeof(Location), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Location>> GetLocationById(int id)
    {
        var location = await _context.Locations.FindAsync(id);

        if (location == null)
            return NotFound();

        return location;
    }

    // POST: api/locations/
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpPost]
    [EndpointSummary("Inserts location into database")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Location>> PostLocation(AddLocationDto dto)
    {
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Code == dto.WarehouseCode);

        if (warehouse == null)
            return Problem(
                title: "Warehouse not found.",
                detail: "Warehouse not found.",
                statusCode: StatusCodes.Status409Conflict);

        var exists = await _context.Locations
            .AnyAsync(x =>
                x.WarehouseId == warehouse.Id &&
                x.Code == dto.Code);

        if (exists)
            return Problem(
                title: "Location with this code already exists in this warehouse.",
                detail: "Location with this code already exists in this warehouse.",
                statusCode: StatusCodes.Status409Conflict);

        var location = new Location
        {
            WarehouseId = warehouse.Id,
            Warehouse = warehouse,
            Code = dto.Code,
            Name = dto.Name,
            IsActive = dto.IsActive
        };

        _context.Locations.Add(location);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetLocationById),
            new { id = location.Id },
            new
            {
                location.Id,
                location.WarehouseId,
                location.Code,
                location.Name,
                location.IsActive
            });
    }

    // PUT: api/locations/5
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpPut("{id}")]
    [EndpointSummary("Updates location in database")]
    [ProducesResponseType(typeof(Location), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Location>> PutLocation(int id, AddLocationDto dto)
    {
        var location = await _context.Locations.FindAsync(id);

        if (location == null)
            return Problem(
                title: "Location doesn't exists.",
                detail: "Location doesn't exists.",
                statusCode: StatusCodes.Status404NotFound);

        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(x => x.Code == dto.WarehouseCode);

        if (warehouse == null)
            return Problem(
                title: "Warehouse not found.",
                detail: "Warehouse not found.",
                statusCode: StatusCodes.Status409Conflict);

        var exists = await _context.Locations
            .AnyAsync(x =>
                x.Id != id &&
                x.WarehouseId == warehouse.Id &&
                x.Code == dto.Code);

        if (exists)
            return Problem(
                title: "Location with this code already exists in this warehouse.",
                detail: "Location with this code already exists in this warehouse.",
                statusCode: StatusCodes.Status409Conflict);

        location.WarehouseId = warehouse.Id;
        location.Warehouse = warehouse;
        location.Code = dto.Code;
        location.Name = dto.Name;
        location.IsActive = dto.IsActive;

        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetLocationById),
            new { id = location.Id },
            new
            {
                location.Id,
                location.WarehouseId,
                location.Code,
                location.Name,
                location.IsActive
            });
    }

    // DELETE: api/locations/5
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpDelete("{id}")]
    [EndpointSummary("Deletes location from database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        var location = await _context.Locations.FindAsync(id);
        if (location == null)
            return NotFound("Location doesn't exists.");

        _context.Locations.Remove(location);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}

