using Warehouse.Domain.Entities;

namespace Warehouse.Application.DTO.User;

public class UserDto
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
}