namespace Warehouse.Domain.Entities;

public class User
{
    public int Id { get; private set; }

    public string Username { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public bool Role { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private User()
    {
    }

    public User(
        string username,
        string passwordHash,
        string email,
        bool role)
    {
        Username = username;
        PasswordHash = passwordHash;
        Email = email;
        Role = role;
        CreatedAt = DateTime.UtcNow;
    }
}