namespace Domain.Entities;

public class User : Entity
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public int FailedLoginAttemts { get; set; } = 0;
    public DateTime? LockoutEnd { get; set; }
    public long StorageQuotaBytes { get; set; } = 5368709120; // 5 GB default quota

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime LastLoginAt { get; set; }

    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetTokenExpiry { get; set; }

    public void UpdateProfile(string firstName, string lastName, string phone, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        Email = email;
        UpdatedAt = DateTime.UtcNow;
    }
}

public class LockoutOptions
{
    public bool AllowebForNewUser { get; set; } = true;
    public int MaxFailedAccessAttempts { get; set; } = 5;
    public TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(5.0);
}
