using Application.AutoMapper;
using Application.DTOs.Request;
using Application.Interfaces;
using Application.Services;
using Application.Validation;
using FluentValidation;
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

        services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();

        services.AddMapster();
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(MapsterConfig).Assembly);

        return services;
    }
}
