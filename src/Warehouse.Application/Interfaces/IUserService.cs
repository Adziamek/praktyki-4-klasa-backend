using System.Security.Claims;
using Warehouse.Application.DTO;
using Warehouse.Application.DTO.Auth;
using Warehouse.Domain.Entities;

namespace Warehouse.Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    UserMeDto? GetMe(ClaimsPrincipal user);
    Task<string?> LoginAsync(LoginUserDto dto);
    Task<RegisterResultDto> SignupAsync(RegisterUserDto dto);
    Task<RegisterResultDto> UpdateAsync(int id, UpdateUserDto dto);
    Task<bool> DeleteAsync(int id);
}