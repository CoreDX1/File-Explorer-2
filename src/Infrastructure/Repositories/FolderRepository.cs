// FileExplorer.Infrastructure/Repositories/FolderRepository.cs
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;

public class FolderRepository : IFolderRepository
{
    private readonly FileExplorerDbContext _context;
    private readonly string _basePath;

    public FolderRepository(FileExplorerDbContext context, IConfiguration configuration)
    {
        _context = context;
        var configPath = configuration["FileStorage:ContainerPath"] ?? "CONTENEDOR";
        _basePath = Path.IsPathRooted(configPath)
            ? configPath
            : Path.Combine(Directory.GetCurrentDirectory(), "..", "..", configPath);
    }

    // File system operations
    public ICollection<DirectoryItem> GetSubFolders(string path)
    {
        // If path is already absolute, use it directly; otherwise combine with basePath
        string fullPath = Path.IsPathRooted(path) ? path : Path.Combine(_basePath, path);

        if (!Directory.Exists(fullPath))
            throw new ArgumentException($"Directory does not exist: {fullPath}");

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
        // If path is already absolute, use it directly; otherwise combine with basePath
        string fullPath = Path.IsPathRooted(path) ? path : Path.Combine(_basePath, path);

        if (!Directory.Exists(fullPath))
            throw new ArgumentException($"Directory does not exist: {fullPath}");

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
        string fullPath = Path.Combine(_basePath, filePath);

        if (!File.Exists(fullPath))
            throw new ArgumentException("File does not exist.");

        return File.ReadAllText(fullPath);
    }

    public bool CreateFolder(string path)
    {
        string fullPath = Path.Combine(_basePath, path);

        if (Directory.Exists(fullPath))
            return false;

        Directory.CreateDirectory(fullPath);
        return true;
    }

    public bool RenameFolder(string oldPath, string newPath)
    {
        string fullPath = Path.Combine(_basePath, oldPath);
        string newFullPath = Path.Combine(_basePath, newPath);

        if (!Directory.Exists(fullPath))
            return false;

        Directory.Move(fullPath, newFullPath);

        return true;
    }

    public bool DeleteFolder(string path)
    {
        string fullPath = Path.Combine(_basePath, path);

        if (!Directory.Exists(fullPath))
            return false;

        Directory.Delete(fullPath, true);
        return true;
    }

    // Database operations
    public IQueryable<FolderItem> Queryable()
    {
        return _context.FolderItems.AsQueryable();
    }

    public async Task<FolderItem?> GetByIdAsync(Guid id)
    {
        return await _context
            .FolderItems.Include(f => f.ParentFolder)
            .Include(f => f.Children)
            .FirstOrDefaultAsync(f => f.Id == id);
    }

    public async Task<ICollection<FolderItem>> GetByParentIdAsync(Guid? parentId)
    {
        return await _context.FolderItems.Where(f => f.ParentFolderId == parentId).ToListAsync();
    }

    public async Task<FolderItem> CreateAsync(FolderItem folderItem)
    {
        // Create physical directory
        string fullPath = Path.Combine(_basePath, folderItem.Path);
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }

        _context.FolderItems.Add(folderItem);
        await _context.SaveChangesAsync();
        return folderItem;
    }

    public async Task UpdateAsync(FolderItem folderItem)
    {
        folderItem.ModifiedAt = DateTime.UtcNow;
        _context.FolderItems.Update(folderItem);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var folderItem = await _context
            .FolderItems.Include(f => f.Children)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (folderItem != null)
        {
            // Delete physical directory if it exists
            if (!string.IsNullOrEmpty(folderItem.Path) && Directory.Exists(folderItem.Path))
            {
                Directory.Delete(folderItem.Path, true);
            }

            _context.FolderItems.Remove(folderItem);
            await _context.SaveChangesAsync();
        }
    }

    public async Task MoveAsync(Guid folderId, Guid newParentId)
    {
        var folderItem = await _context.FolderItems.FindAsync(folderId);
        if (folderItem == null)
            throw new KeyNotFoundException($"Folder with ID {folderId} not found");

        var newParent = await _context.FolderItems.FindAsync(newParentId);
        if (newParent == null && newParentId != Guid.Empty)
            throw new KeyNotFoundException($"Parent folder with ID {newParentId} not found");

        folderItem.ParentFolderId = newParentId == Guid.Empty ? null : newParentId;
        folderItem.ModifiedAt = DateTime.UtcNow;

        _context.FolderItems.Update(folderItem);
        await _context.SaveChangesAsync();
    }

    public async Task<ICollection<FolderItem>> GetSubFoldersByParentIdAsync(Guid? parentId)
    {
        return await _context.FolderItems.Where(f => f.ParentFolderId == parentId).ToListAsync();
    }

    public async Task<ICollection<FileItem>> GetFilesByFolderIdAsync(Guid folderId)
    {
        return await _context.FileItems.Where(f => f.ParentFolderId == folderId).ToListAsync();
    }
}
