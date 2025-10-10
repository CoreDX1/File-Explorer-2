using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Interface;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbContext<FileExplorerDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("DefaultConection"));
        });

        // Unit of Work registration
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();

        // Repository registration (optional if using Unit of Work)
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFolderRepository, FolderRepository>();
    }
}
