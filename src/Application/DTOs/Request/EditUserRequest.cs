namespace Application.DTOs.Request;

public sealed record EditUserRequest(
    int Id,
    string FirstName = "",
    string LastName = "",
    string Phone = "",
    string Password = "",
    string Email = ""
);
