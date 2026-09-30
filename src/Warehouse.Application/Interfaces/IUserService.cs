using System.Security.Claims;
using Warehouse.Application.Common;
using Warehouse.Application.DTO.User;

namespace Warehouse.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserResponseDto>> GetAllAsync();
    Task<UserResponseDto?> GetByIdAsync(int id);

    Task<OperationResult<UserResponseDto>> AddAsync(RegisterUserDto dto);
    Task<OperationResult<UserResponseDto>> UpdateAsync(int id, UserDto dto);

    Task<OperationResult> DeleteAsync(int id);

    Task<string?> LoginAsync(LoginUserDto dto);
    UserMeDto? GetMe(ClaimsPrincipal user);
}