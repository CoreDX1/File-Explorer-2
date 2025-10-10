namespace Application.Services;

using Application.Interface;
using Ardalis.Result;
using Domain.Entities;
using Domain.Interfaces;

public class FolderServices : IFolderServices
{
    private readonly IFolderRepository _folderRepository;

    public FolderServices(IFolderRepository folderRepository)
    {
        _folderRepository = folderRepository;
    }

    public Result<List<DirectoryItem>> GetSubFolders(string path)
    {
        return _folderRepository.GetSubFolders(path);
    }

    public Result<List<FileItem>> GetFiles(string path)
    {
        return _folderRepository.GetFiles(path);
    }

    public Result<string> ReadFile(string filePath)
    {
        var file = _folderRepository.ReadFile(filePath);

        return Result.Success(file, "El archivo fue leido correctamente.");
    }

    public Result<string> CreateFolder(string path)
    {
        var folder = _folderRepository.CreateFolder(path);

        if (!folder)
            return Result.NotFound("El directorio no pudo ser creado.");

        return Result.Success("El directorio fue creado correctamente.");
    }

    public Result<string> RenameFolder(string oldPath, string newPath)
    {
        var folder = _folderRepository.RenameFolder(oldPath, newPath);

        if (!folder)
            return Result.NotFound("El directorio no pudo ser renombrado.");

        return Result.Success("El directorio fue renombrado correctamente.");
    }

    public Result<string> DeleteFolder(string path)
    {
        var folder = _folderRepository.DeleteFolder(path);

        if (!folder)
            return Result.NotFound("El directorio no pudo ser borrado.");

        return Result.Success("El directorio fue borrado correctamente.");
    }
}
