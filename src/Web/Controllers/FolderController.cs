namespace Web.Controllers;

using Application.Services.Interfaces;
using Ardalis.Result;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class FolderController : ControllerBase
{
    private readonly IFolderServices _folderServices;

    public FolderController(IFolderServices folderServices)
    {
        _folderServices = folderServices;
    }

    [HttpGet]
    [Route("list")] // GET /api/folder/list
    public Result<List<DirectoryItem>> GetSubFolders(string path)
    {
        return _folderServices.GetSubFolders(path);
    }

    [HttpGet]
    [Route("{path}")] // GET /api/folder/list
    public Result<List<FileItem>> GetFiles(string path)
    {
        return _folderServices.GetFiles(path);
    }

    [HttpPut("rename")] // PUT /api/folder/rename
    public Result<string> RenameFolder(string oldPath, string newPath)
    {
        return _folderServices.RenameFolder(oldPath, newPath);
    }

    [HttpDelete("delete")] // DELETE /api/folder/delete
    public Result<string> DeleteFolder(string path)
    {
        return _folderServices.DeleteFolder(path);
    }

    [HttpPost("create")] // POST /api/folder/create
    public Result<string> CreateFolder(string path)
    {
        return _folderServices.CreateFolder(path);
    }
}