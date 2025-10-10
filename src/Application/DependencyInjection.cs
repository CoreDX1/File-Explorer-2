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
        return services;
    }
}
