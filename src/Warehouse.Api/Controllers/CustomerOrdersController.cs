using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.DTO.CustomerOrder;
using Warehouse.Application.Interfaces;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerOrdersController : ControllerBase
{
    private readonly ICustomerOrderService _customerOrderService;
    private readonly ILogger<CustomerOrdersController> _logger;

    public CustomerOrdersController(
        ICustomerOrderService customerOrderService,
        ILogger<CustomerOrdersController> logger)
    {
        _customerOrderService = customerOrderService;
        _logger = logger;
    }

    // GET: api/orders
    [HttpGet]
    [EndpointSummary("Customer Order list")]
    [ProducesResponseType(typeof(IReadOnlyList<CustomerOrderResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CustomerOrderResponseDto>>> GetCustomerOrders()
    {
        var customerOrders = await _customerOrderService.GetAllAsync();
        return Ok(customerOrders);
    }

    // GET: api/orders/{id}
    [HttpGet("{id:int}")]
    [EndpointSummary("Returns customer order by id")]
    [ProducesResponseType(typeof(CustomerOrderResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerOrderResponseDto>> GetCustomerOrderById(int id)
    {
        var customerOrder = await _customerOrderService.GetByIdAsync(id);

        if (customerOrder is null)
            return NotFound();

        return Ok(customerOrder);
    }

    // POST: api/orders
    [HttpPost]
    [EndpointSummary("Creates new customer order")]
    [ProducesResponseType(typeof(CustomerOrderResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CustomerOrderResponseDto>> AddCustomerOrder(CustomerOrderDto dto)
    {
        var result = await _customerOrderService.AddAsync(dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        var customerOrder = result.CustomerOrder!;

        return CreatedAtAction(
            nameof(GetCustomerOrderById),
            new { id = customerOrder.Id },
            customerOrder);
    }
}