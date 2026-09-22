using Microsoft.AspNetCore.Mvc;
using Warehouse.Domain.Entities;
using Warehouse.Application.DTO;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Warehouse.Infrastructure.Data;
using Warehouse.Application.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly WarehouseDbContext _context;
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;
    public UsersController(
        WarehouseDbContext context,
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _context = context;
        _userService = userService;
        _logger = logger;
    }

    // GET: api/User/5
    [HttpGet]
    [EndpointSummary("Returns list")]
    [ProducesResponseType(typeof(IReadOnlyList<User>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<User>>> GetUsers()
    {
        var user = _userService.GetAllAsync();
        return Ok(user);
    }

    // GET: api/User/5
    [HttpGet("{id}")]
    [EndpointSummary("Returns user by id")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = _userService.GetByIdAsync(id);

        if (user == null)
            return NotFound();

        return Ok(user);
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var tokenRespone = _userService.GetMe(User);
        
        return Ok(tokenRespone);
    }

    // POST: api/User/login
    [HttpPost("login")]
    [EndpointSummary("Checks user username and password")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<string>> Login(LoginUserDto dto)
    {
        var token = _userService.LoginAsync(dto);

        if (token == null)
            return Problem(
                title: "Invalid credentials",
                detail: "Wrong username or password",
                statusCode: StatusCodes.Status401Unauthorized
            );

        return Ok(new
        {
            Token = token
        });
    }

    // POST: api/User
    [HttpPost("signup")]
    [EndpointSummary("Inserts user into database")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<User>> PostUser(RegisterUserDto dto)
    {
        var result = await _userService.SignupAsync(dto);

        if (result.StatusCode != null)
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);

        var user = result!.User;

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = user.Id },
            new
            {
                user.Id,
                user.Username,
                user.Role,
                user.Email
            });
    }

    // DELETE: api/User/5
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpDelete("{id}")]
    [EndpointSummary("Deletes user from database")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var deleted = await _userService.DeleteAsync(id);
        
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}