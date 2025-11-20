namespace Domain.Entities;

public class DirectoryItem : FileSystemItem
{
    public ICollection<FileItem> Files { get; } = new List<FileItem>();
    public ICollection<DirectoryItem> SubFolders { get; } = new List<DirectoryItem>();

    public DirectoryItem(
        string name,
        string path,
        long size,
        DateTime createdAt,
        DateTime modifiedAt
    )
        : base(name, path, size, createdAt, modifiedAt)
    {
        IsDirectory = true;
        ItemType = FileSystemItemType.Directory;
    }
}
