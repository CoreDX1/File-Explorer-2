using Ardalis.Result;
using Domain.Entities;

namespace Application.Interface;

public interface IFolderServices
{
    Result<List<DirectoryItem>> GetSubFolders(string path);
    Result<List<FileItem>> GetFiles(string path);
    Result<string> ReadFile(string filePath);

    Result<string> CreateFolder(string path);
    Result<string> RenameFolder(string oldPath, string newPath);
    Result<string> DeleteFolder(string path);
}
