using Microservices.Shared.BuildingBlocks.Domain;

namespace IdentityService.Domain;

public sealed class User : AggregateRoot
{
    private User() { }

    public Guid Id { get; private set; }
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public string Role { get; private set; } = Roles.User;
    public DateTime CreatedAt { get; private set; }

    public static User Register(Guid id, Email email, string passwordHash, string role)
    {
        var user = new User
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = DateTime.UtcNow
        };

        user.AddDomainEvent(new UserRegisteredDomainEvent(id, email.Value, role));
        return user;
    }

    public static User Load(Guid id, Email email, string passwordHash, string role, DateTime createdAt)
    {
        return new User
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            CreatedAt = createdAt
        };
    }

    public void ChangeRole(string role)
    {
        Role = role;
    }
}

public static class Roles
{
    public const string User = "User";
    public const string Admin = "Admin";
}
