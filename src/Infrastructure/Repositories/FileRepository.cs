using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class FileRepository : IFileRepository
{
    public List<FileItem> GetFilesAsync(string path)
    {
        string fullPath = Path.Combine("/home/christian/Desktop/Projects/File-Explorer/CONTENEDOR", path);

        if (!Directory.Exists(fullPath))
            throw new ArgumentException("El directorio no existe.");

        var files = new List<FileItem>();

        foreach (var file in Directory.GetFiles(fullPath))
        {
            var fileInfo = new FileInfo(file);
            files.Add(new FileItem(
                name: Path.GetFileName(file),
                path: file,
                size: fileInfo.Length,
                createdAt: fileInfo.CreationTime,
                modifiedAt: fileInfo.LastWriteTime
            ));
        }

        return files;
    }

    public string ReadFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }
}