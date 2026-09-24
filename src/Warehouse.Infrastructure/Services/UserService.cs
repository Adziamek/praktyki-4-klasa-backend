using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Warehouse.Application.DTO.User;
using Warehouse.Application.Interfaces;
using Warehouse.Domain.Entities;
using Warehouse.Infrastructure.Data;

namespace Warehouse.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly WarehouseDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public UserService(
        WarehouseDbContext context,
        IPasswordHasher<User> passwordHasher,
        IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    private static UserResponseDto MapToDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt
        };
    }

    public async Task<IReadOnlyList<UserResponseDto>> GetAllAsync()
    {
        var users = await _context.Users
            .AsNoTracking()
            .ToListAsync();

        return users
            .Select(MapToDto)
            .ToList();
    }

    public async Task<UserResponseDto?> GetByIdAsync(int id)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        return user is null
            ? null
            : MapToDto(user);
    }

    public async Task<UserOperationResult> AddAsync(
        RegisterUserDto dto)
    {
        var existingUsername = await _context.Users
            .AnyAsync(x => x.Username == dto.Username);

        if (existingUsername)
        {
            return UserOperationResult.Fail(
                "Username already exists.",
                StatusCodes.Status409Conflict);
        }

        var existingEmail = await _context.Users
            .AnyAsync(x => x.Email == dto.Email);

        if (existingEmail)
        {
            return UserOperationResult.Fail(
                "User with this email already exists.",
                StatusCodes.Status409Conflict);
        }

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            Role = UserRole.User,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(
            user,
            dto.Password);

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return UserOperationResult.Ok(
            MapToDto(user));
    }

    public async Task<UserOperationResult> UpdateAsync(
        int id,
        UserDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
        {
            return UserOperationResult.Fail(
                "User does not exist.",
                StatusCodes.Status404NotFound);
        }

        var existingUsername = await _context.Users
            .AnyAsync(x =>
                x.Username == dto.Username &&
                x.Id != id);

        if (existingUsername)
        {
            return UserOperationResult.Fail(
                "Username already exists.",
                StatusCodes.Status409Conflict);
        }

        var existingEmail = await _context.Users
            .AnyAsync(x =>
                x.Email == dto.Email &&
                x.Id != id);

        if (existingEmail)
        {
            return UserOperationResult.Fail(
                "User with this email already exists.",
                StatusCodes.Status409Conflict);
        }

        user.Username = dto.Username;
        user.Email = dto.Email;
        user.Role = dto.Role;

        await _context.SaveChangesAsync();

        return UserOperationResult.Ok(
            MapToDto(user));
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Id == id);

        if (user == null)
            return false;

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<string?> LoginAsync(LoginUserDto dto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x =>
                x.Username == dto.Username);

        if (user == null)
            return null;

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            dto.Password);

        if (result == PasswordVerificationResult.Failed)
            return null;

        var claims = new[]
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()),

            new Claim(
                "username",
                user.Username),

            new Claim(
                ClaimTypes.Role,
                user.Role.ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    public UserMeDto? GetMe(ClaimsPrincipal user)
    {
        var id = user.FindFirstValue(
            ClaimTypes.NameIdentifier);

        var username = user.FindFirstValue(
            "username");

        var role = user.FindFirstValue(
            ClaimTypes.Role);

        if (id == null ||
            username == null ||
            role == null)
        {
            return null;
        }

        return new UserMeDto
        {
            Id = int.Parse(id),
            Username = username,
            Role = role
        };
    }
}
