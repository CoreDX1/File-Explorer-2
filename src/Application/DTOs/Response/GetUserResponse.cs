namespace Application.DTOs.Response;

public sealed record GetUserResponse(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime LastLoginAt
)
{
    public string FullName => $"{FirstName} {LastName}";
};
