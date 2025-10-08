using Domain.Entities;

namespace Domain.Interfaces;

public interface IFolderRepository
{
    public List<DirectoryItem> GetSubFolders(string path);
    public List<FileItem> GetFiles(string path);
    public string ReadFile(string filePath);

    public bool CreateFolder(string path);
    public bool RenameFolder(string oldPath, string newPath);
    public bool DeleteFolder(string path);
}