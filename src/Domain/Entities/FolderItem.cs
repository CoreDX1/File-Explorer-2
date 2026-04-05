namespace Domain.Entities;

public class FolderItem : FileSystemItem
{
    public virtual ICollection<FileSystemItem>? Children { get; set; }

    public FolderItem(string name, string path, long size, DateTime createdAt, DateTime modifiedAt)
        : base(name, path, size, createdAt, modifiedAt)
    {
        IsDirectory = true;
        ItemType = FileSystemItemType.Directory;
    }
}
