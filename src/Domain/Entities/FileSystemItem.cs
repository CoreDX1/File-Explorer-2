namespace Domain.Entities;

public abstract class FileSystemItem
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool IsDirectory { get; protected set; }
    public FileSystemItemType ItemType { get; protected set; }

    protected FileSystemItem(string name, string path, long size, DateTime createdAt, DateTime modifiedAt)
    {
        Name = name;
        Path = path;
        Size = size;
        CreatedAt = createdAt;
        ModifiedAt = modifiedAt;
    }
}

public enum FileSystemItemType
{
    File,
    Directory
}