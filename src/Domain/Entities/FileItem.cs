namespace Domain.Entities;

public class FileItem : FileSystemItem
{
    public FileItem(string name, string path, long size, DateTime createdAt, DateTime modifiedAt)
        : base(name, path, size, createdAt, modifiedAt)
    {
        IsDirectory = false;
        ItemType = FileSystemItemType.File;
    }
}