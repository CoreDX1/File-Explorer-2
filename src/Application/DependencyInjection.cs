using Application.Interface;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IFileServices, FileServices>();
        services.AddScoped<IFolderServices, FolderServices>();
        services.AddScoped<IUserServices, UserServices>();

        services.AddAutoMapper(typeof(DependencyInjection)); // Busca perfiles en este assembly

        return services;
    }
}
