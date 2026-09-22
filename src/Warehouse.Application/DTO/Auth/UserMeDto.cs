using Warehouse.Domain.Entities;

namespace Warehouse.Application.DTO.Auth;
public class UserMeDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}