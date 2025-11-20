using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IFolderService
    {
        Task RenameFolderAsync(string oldPath, string newPath);
        Task DeleteFolderAsync(string path);
    }
}
