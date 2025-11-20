using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Interfaces;
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
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        });

        // Register DbContext base class for UnitOfWork
        services.AddScoped<DbContext>(provider => provider.GetService<FileExplorerDbContext>()!);

        // Unit of Work registration
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();

        // Repository registration (optional if using Unit of Work)
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFolderRepository, FolderRepository>();

        // Generic repository registration
        services.AddScoped(typeof(IRepositoryAsync<>), typeof(Repository<>));
    }
}
