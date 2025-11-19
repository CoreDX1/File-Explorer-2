namespace Application.DTOs.Response;

public sealed record GetUserResponseUnique(
    int Id,
    string FirstName,
    string LastName,
    string Email,
    string Phone
);
