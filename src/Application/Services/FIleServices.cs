using Application.Interface;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class FileServices : IFileServices
{
    private readonly IFileRepository _fileRepository;

    public FileServices(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public Task<ShareLinkResult> CreateShareLinkAsync(Guid id, ShareFileRequest request)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFileAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<FileDownloadResult?> DownloadFileAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<FileItem?> GetFileByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public List<FileItem> GetFiles(string path)
    {
        return _fileRepository.GetFilesAsync(path);
    }

    public string ReadFile(string filePath)
    {
        return _fileRepository.ReadFileAsync(filePath);
    }

    public Task<FileUploadResult> UploadFileAsync(IFormFile file, Guid? folderId)
    {
        throw new NotImplementedException();
    }
}
