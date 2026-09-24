namespace Warehouse.Domain.Entities;

public enum UserRole
{
    User,
    Warehouseman,
    Administrator
}

public class User
{
    public int Id { get; set; }

    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public DateTime CreatedAt { get; set; }

    public ICollection<CustomerOrder> Orders { get; set; }
        = new List<CustomerOrder>();
}