// FileExplorer.Infrastructure/Repositories/FolderRepository.cs
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class FolderRepository : IFolderRepository
{
    private string FullPath = "/home/christian/Desktop/Projects/File-Explorer/CONTENEDOR";

    public ICollection<DirectoryItem> GetSubFolders(string path)
    {
        string fullPath = Path.Combine(FullPath, path);

        if (!Directory.Exists(fullPath))
            throw new ArgumentException("Directory does not exist.");

        var subFolders = new List<DirectoryItem>();

        foreach (var dir in Directory.GetDirectories(fullPath))
        {
            subFolders.Add(
                new DirectoryItem(
                    name: Path.GetFileName(dir),
                    path: dir,
                    size: new DirectoryInfo(dir).EnumerateFiles().Sum(file => file.Length),
                    createdAt: new FileInfo(dir).CreationTime,
                    modifiedAt: new FileInfo(dir).LastWriteTime
                )
            );
        }

        return subFolders;
    }

    public ICollection<FileItem> GetFiles(string path)
    {
        string fullPath = Path.Combine(FullPath, path);

        if (!Directory.Exists(fullPath))
            throw new ArgumentException("Directory does not exist.");

        var files = new List<FileItem>();

        foreach (var file in Directory.GetFiles(fullPath))
        {
            var fileInfo = new FileInfo(file);
            files.Add(
                new FileItem(
                    name: Path.GetFileName(file),
                    path: file,
                    size: fileInfo.Length,
                    createdAt: fileInfo.CreationTime,
                    modifiedAt: fileInfo.LastWriteTime
                )
            );
        }

        return files;
    }

    public string ReadFile(string filePath)
    {
        string fullPath = Path.Combine(FullPath, filePath);

        if (!File.Exists(fullPath))
            throw new ArgumentException("File does not exist.");

        return File.ReadAllText(fullPath);
    }

    public bool RenameFolder(string oldPath, string newPath)
    {
        string fullPath = Path.Combine(FullPath, oldPath);
        string newFullPath = Path.Combine(FullPath, newPath);

        if (!Directory.Exists(fullPath))
            return false;

        Directory.Move(fullPath, newFullPath);

        return true;
    }

    public bool DeleteFolder(string path)
    {
        string fullPath = Path.Combine(FullPath, path);

        if (!Directory.Exists(fullPath))
            return false;

        Directory.Delete(fullPath, true);
        return true;
    }

    public bool CreateFolder(string path)
    {
        string fullPath = Path.Combine(FullPath, path);

        if (Directory.Exists(fullPath))
            return false;

        Directory.CreateDirectory(fullPath);
        return true;
    }
}
