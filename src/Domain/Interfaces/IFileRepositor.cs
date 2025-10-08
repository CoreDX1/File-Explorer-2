using Domain.Entities;

namespace Domain.Interfaces;

public interface IFileRepository
{
    public List<FileItem> GetFilesAsync(string path);
    public string ReadFileAsync(string filePath);
}