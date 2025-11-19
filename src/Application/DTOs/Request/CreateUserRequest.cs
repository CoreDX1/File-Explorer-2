namespace Application.DTOs.Request;

public sealed record CreateUserRequest(
    string FirstName,
    string LastName,
    string Phone,
    string Password,
    string Email
);
