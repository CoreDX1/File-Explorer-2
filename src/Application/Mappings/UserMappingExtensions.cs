using Application.DTOs.Response;
using Domain.Entities;

namespace Application.Mappings;

public static class UserMappingExtensions
{
    public static UserResponse ToDto(this User user)
    {
        return new(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Phone,
            user.IsActive,
            user.CreatedAt,
            user.UpdatedAt,
            user.LastLoginAt
        );
    }

    public static IEnumerable<UserResponse> ToDtos(this IEnumerable<User> users)
    {
        return users.Select(u => u.ToDto());
    }
}
