using Application.AutoMapper;
using Application.Interface;
using Application.Services;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IFileServices, FileServices>();
        services.AddScoped<IFolderServices, FolderServices>();
        services.AddScoped<IUserServices, UserServices>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();

        services.AddMapster();
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MapsterConfig).Assembly);

        return services;
    }
}
