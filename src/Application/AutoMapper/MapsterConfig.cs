using Application.DTOs.Request;
using Domain.Entities;
using Mapster;

namespace Application.AutoMapper;

public class MapsterConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config
            .NewConfig<CreateUserRequest, User>()
            .Map(dest => dest.Username, src => src.UserName)
            .Map(dest => dest.PasswordHash, () => (string?)null)
            .Map(dest => dest.CreatedAt, () => DateTime.UtcNow)
            .Map(dest => dest.UpdatedAt, () => DateTime.UtcNow)
            .Map(dest => dest.FullName, src => $"{src.UserName} {src.LastName}")
            .Map(dest => dest.IsActive, () => true)
            .Map(dest => dest.LastLoginAt, () => (DateTime?)null)
            .Map(dest => dest.StorageQuotaBytes, () => 1073741824L);
    }
}
