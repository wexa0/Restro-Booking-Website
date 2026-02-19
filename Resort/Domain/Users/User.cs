namespace Resort.Domain.Users;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");

    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
