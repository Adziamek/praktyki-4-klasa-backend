using System.Security.Claims;
using Warehouse.Application.DTO.User;

namespace Warehouse.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByIdAsync(int id);

    Task<UserOperationResult> AddAsync(RegisterUserDto dto);
    Task<UserOperationResult> UpdateAsync(int id, UserDto dto);

    Task<bool> DeleteAsync(int id);

    Task<string?> LoginAsync(LoginUserDto dto);
    UserMeDto? GetMe(ClaimsPrincipal user);
}
public record UserOperationResult(
    UserResponseDto? User,
    string? Error,
    int? StatusCode)
{
    public bool Success => User != null;

    public static UserOperationResult Ok(UserResponseDto user) =>
        new(user, null, null);

    public static UserOperationResult Fail(
        string error,
        int statusCode) =>
        new(null, error, statusCode);
}
