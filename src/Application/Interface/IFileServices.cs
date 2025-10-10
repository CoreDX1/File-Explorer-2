using Domain.Entities;

namespace Application.Interface;

public interface IFileServices
{
    List<FileItem> GetFiles(string path);
    string ReadFile(string filePath);
}
