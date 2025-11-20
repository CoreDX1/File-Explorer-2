using Application.Interfaces;
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

    public Task<bool> CreateFileAsync(CreateFileRequest createFile)
    {
        throw new NotImplementedException();
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

    public ICollection<FileItem> GetFiles(string path)
    {
        return _fileRepository.GetFilesAsync(path);
    }

    public string ReadFile(string filePath)
    {
        return _fileRepository.ReadFileAsync(filePath);
    }

    public async Task<FileUploadResult> UploadFileAsync(IFormFile file, Guid? folderId)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is required", nameof(file));
        }

        if (file.Length > 100 * 1024 * 1024) // 100 MB limit
        {
            throw new ArgumentException("File size exceeds the 100 MB limit", nameof(file));
        }

        var fileId = Guid.NewGuid();
        var fileName = Path.GetFileNameWithoutExtension(file.FileName);
        var extension = Path.GetExtension(file.FileName);
        var contentType = file.ContentType;

        var storagePath = $"uploads/{DateTime.UtcNow:yyyy/MM/dd}/{fileId}{extension}";
        var physicalPath = Path.Combine("wwwroot", storagePath);

        Directory.CreateDirectory(Path.GetDirectoryName(physicalPath)!);

        using (var stream = new FileStream(physicalPath, FileMode.Create))
        {
            await file.CopyToAsync(stream).ConfigureAwait(false);
        }

        var fileEntity = new FileItem(
            file.FileName,
            physicalPath,
            file.Length,
            DateTime.UtcNow,
            DateTime.UtcNow
        );

        // TODO: Implement file entity persistence
        // For now, just save the physical file
        return new FileUploadResult(fileId, file.FileName, file.Length);
    }
}
