namespace Application.Configuration;

public class LockoutOptions
{
    public int MaxFailedAccessAttempts { get; set; } = 5;
    public int DefaultLockoutTimeSpan { get; set; } = 15;
    public TimeSpan LockoutTimeSpan => TimeSpan.FromMinutes(DefaultLockoutTimeSpan);
}
