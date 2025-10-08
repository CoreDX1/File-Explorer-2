namespace Domain.Entities;

public class DirectoryItem : FileSystemItem
{
    public List<FileItem> Files { get; set; } = new List<FileItem>();
    public List<DirectoryItem> SubFolders { get; set; } = new List<DirectoryItem>();

    public DirectoryItem(string name, string path, long size, DateTime createdAt, DateTime modifiedAt)
        : base(name, path, size, createdAt, modifiedAt)
    {
        IsDirectory = true;
        ItemType = FileSystemItemType.Directory;
    }
}