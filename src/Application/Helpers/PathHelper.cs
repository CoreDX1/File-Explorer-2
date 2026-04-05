using Microsoft.Extensions.Configuration;

namespace Application.Helpers;

public static class PathHelper
{
    public static string ResolveBasePath(
        IConfiguration configuration,
        string configKey = "FileStorage:ContainerPath",
        string defaultPath = "CONTENEDOR"
    )
    {
        var configPath = configuration[configKey] ?? defaultPath;
        return Path.IsPathRooted(configPath)
            ? configPath
            : Path.Combine(Directory.GetCurrentDirectory(), "..", "..", configPath);
    }

    public static string ResolveFullPath(string basePath, string path)
    {
        return Path.IsPathRooted(path) ? path : Path.Combine(basePath, path);
    }
}
