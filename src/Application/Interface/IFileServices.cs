using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interface;

public interface IFileServices
{
    public List<FileItem> GetFiles(string path);
    public string ReadFile(string filePath);

    // New methods for FileController
    Task<FileItem?> GetFileByIdAsync(Guid id);
    Task<FileUploadResult> UploadFileAsync(IFormFile file, Guid? folderId);
    Task<FileDownloadResult?> DownloadFileAsync(Guid id);
    Task DeleteFileAsync(Guid id);
    Task<ShareLinkResult> CreateShareLinkAsync(Guid id, ShareFileRequest request);
}

public record FileUploadResult(Guid Id, string FileName, long Size);

public record FileDownloadResult(Stream Stream, string ContentType, string FileName);

public record ShareLinkResult(string ShareUrl, DateTime ExpiresAt);

public record ShareFileRequest(string Email, string Permission, DateTime? ExpiresAt);
