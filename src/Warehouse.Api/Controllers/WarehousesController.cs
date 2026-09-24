using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Warehouse.Application.DTO.Warehouse;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

[Route("api/[controller]")]
[ApiController]
public class WarehousesController : ControllerBase
{
    private readonly WarehouseDbContext _context;
    private readonly ILogger<WarehousesController> _logger;
    public WarehousesController(
        WarehouseDbContext context,
        ILogger<WarehousesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/warehouses
    [HttpGet]
    [EndpointSummary("Returns all warehouses")]
    [ProducesResponseType(typeof(IEnumerable<Warehouse.Domain.Entities.CWarehouse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Warehouse.Domain.Entities.CWarehouse>>> GetWarehouses()
    {
        var warehouses = await _context.Warehouses.Select(x => new Warehouse.Domain.Entities.CWarehouse
        {
            Id = x.Id,
            Code = x.Code,
            Name = x.Name,
            Active = x.Active
        })
        .OrderBy(x => x.Code)
        .ToListAsync();

        return Ok(warehouses);
    }

    // GET: api/warehouses/5
    [HttpGet("{id}")]
    [EndpointSummary("Returns warehouse by id")]
    [ProducesResponseType(typeof(Warehouse.Domain.Entities.CWarehouse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Warehouse.Domain.Entities.CWarehouse>> GetWarehouseById(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);

        if (warehouse == null)
            return Problem(
                title: "Warehouse doesn't exist.",
                detail: "Warehouse doesn't exist.",
                statusCode: StatusCodes.Status404NotFound);

        return warehouse;
    }

    // POST: api/warehouses
    //[Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpPost]
    [EndpointSummary("Adds warehouse to database")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Warehouse.Domain.Entities.CWarehouse>> PostWarehouse(AddWarehouseDto dto)
    {
        var exists = await _context.Warehouses.AnyAsync(x => x.Code == dto.Code);

        if (exists)
            return Problem(
                title: "Warehouse with this code already exists.",
                detail: "Warehouse with this code already exists.",
                statusCode: StatusCodes.Status409Conflict);

        var warehouse = new Warehouse.Domain.Entities.CWarehouse
        {
            Code = dto.Code,
            Name = dto.Name,
            Active = dto.IsActive
        };

        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetWarehouseById),
            new { id = warehouse.Id },
            new
            {
                warehouse.Code,
                warehouse.Name,
                IsActive = warehouse.Active
            });
    }

    // PUT: api/warehouses/5
    //[Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpPut("{id}")]
    [EndpointSummary("Updates warehouse in database")]
    [ProducesResponseType(typeof(Warehouse.Domain.Entities.CWarehouse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Warehouse.Domain.Entities.CWarehouse>> PutWarehouse(int id, AddWarehouseDto dto)
    {
        var exists = await _context.Warehouses.AnyAsync(x => x.Id != id && x.Code == dto.Code);

        if (exists)
            return Problem(
                title: "Warehouse already exists.",
                detail: "Warehouse with this code or name already exists.",
                statusCode: StatusCodes.Status409Conflict);

        var warehouse = await _context.Warehouses.FindAsync(id);

        if (warehouse == null)
            return Problem(
                title: "Warehouse doesn't exists.",
                detail: "Warehouse doesn't exists.",
                statusCode: StatusCodes.Status404NotFound);

        warehouse.Code = dto.Code;
        warehouse.Name = dto.Name;
        warehouse.Active = dto.IsActive;

        await _context.SaveChangesAsync();

        return Ok(warehouse);
    }

    // DELETE: api/warehouses/5
    //[Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpDelete("{id}")]
    [EndpointSummary("Deletes warehouse from database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteWarehouse(int id)
    {
        var warehouse = await _context.Warehouses.FindAsync(id);
        if (warehouse == null)
            return NotFound("Warehouse doesn't exists.");

        _context.Warehouses.Remove(warehouse);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}