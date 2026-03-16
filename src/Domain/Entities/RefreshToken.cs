namespace Domain.Entities;

public class RefreshToken : Entity
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expire { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? ReasonRevoked { get; set; }

    public bool IsExpired => DateTime.UtcNow >= Expire;
    public bool IsRevoked => Revoked.HasValue;
    public bool IsActive => !IsRevoked && !IsExpired;

    public User? User { get; set; }
}
