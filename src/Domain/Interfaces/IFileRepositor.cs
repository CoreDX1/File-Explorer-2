using Domain.Entities;

namespace Domain.Interfaces;

public interface IFileRepository
{
    public ICollection<FileItem> GetFilesAsync(string path);
    public string ReadFileAsync(string filePath);
}