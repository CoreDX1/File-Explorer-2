using Domain.Entities;

namespace Application.Services.Interfaces;

public interface IFileServices
{
    List<FileItem> GetFiles(string path);
    string ReadFile(string filePath);
}
