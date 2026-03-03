namespace Application.DTOs.Response;

public sealed record LoginResponse(
    string Email,
    string FirstName,
    string LastName,
    string? Phone,
    string AccessToken,
    string? RefreshToken
);
