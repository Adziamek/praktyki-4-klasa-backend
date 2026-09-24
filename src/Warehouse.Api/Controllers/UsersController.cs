using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Warehouse.Application.DTO.User;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;

namespace Warehouse.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // GET: api/users
    [HttpGet]
    [EndpointSummary("User list")]
    [ProducesResponseType(
        typeof(IReadOnlyList<UserResponseDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserResponseDto>>>
        GetUsers()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    // GET: api/users/{id}
    [HttpGet("{id}")]
    [EndpointSummary("Returns user by id")]
    [ProducesResponseType(
        typeof(UserResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>>
        GetUserById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user is null)
            return NotFound();

        return Ok(user);
    }

    // GET: api/users/me
    [Authorize]
    [HttpGet("me")]
    [EndpointSummary("Returns currently logged in user")]
    [ProducesResponseType(
        typeof(UserMeDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public IActionResult GetMe()
    {
        var user = _userService.GetMe(User);

        if (user is null)
            return Unauthorized();

        return Ok(user);
    }

    // POST: api/users/login
    [HttpPost("login")]
    [EndpointSummary("Checks username and password")]
    [ProducesResponseType(
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        LoginUserDto dto)
    {
        var token = await _userService.LoginAsync(dto);

        if (token is null)
        {
            return Problem(
                title: "Invalid credentials",
                detail: "Wrong username or password.",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        return Ok(new
        {
            Token = token
        });
    }

    // POST: api/users/signup
    [HttpPost("signup")]
    [EndpointSummary("Creates new user")]
    [ProducesResponseType(
        typeof(UserResponseDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponseDto>>
        Signup(RegisterUserDto dto)
    {
        _logger.LogDebug(
            "Registering user: {Username}, Email: {Email}",
            dto.Username,
            dto.Email);

        var result = await _userService.AddAsync(dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        var user = result.User!;

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = user.Id },
            user);
    }

    // PUT: api/users/{id}
    [Authorize(Roles = nameof(UserRole.Administrator))]
    [HttpPut("{id}")]
    [EndpointSummary("Updates existing user")]
    [ProducesResponseType(
        typeof(UserResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    [ProducesResponseType(
        StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponseDto>>
        UpdateUser(
            int id,
            UserDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);

        if (!result.Success)
        {
            return Problem(
                title: result.Error,
                detail: result.Error,
                statusCode: result.StatusCode);
        }

        return Ok(result.User);
    }
}
