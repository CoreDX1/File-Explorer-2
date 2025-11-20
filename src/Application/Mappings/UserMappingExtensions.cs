using Application.DTOs.Response;
using Domain.Entities;

namespace Application.Mappings;

public static class UserMappingExtensions
{
    public static UserResponse ToDto(this User user)
    {
        return new(user.Id, user.FirstName, user.LastName, user.Email, user.Phone);
    }

    public static List<UserResponse> ToDtos(this List<User> users)
    {
        return users.Select(u => u.ToDto()).ToList();
    }
}
