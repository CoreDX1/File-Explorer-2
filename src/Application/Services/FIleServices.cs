using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class FileServices : IFileServices
{
    private readonly IFileRepository _fileRepository;

    public FileServices(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }

    public List<FileItem> GetFiles(string path)
    {
        return _fileRepository.GetFilesAsync(path);
    }

    public string ReadFile(string filePath)
    {
        return _fileRepository.ReadFileAsync(filePath);
    }
}
