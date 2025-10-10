using Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileServices _fileServices;

    public FileController(IFileServices fileServices)
    {
        _fileServices = fileServices;
    }

    [HttpGet]
    [Route("list")]
    public IActionResult GetFiles(string path)
    {
        var files = _fileServices.GetFiles(path);

        return Ok(files);
    }

    [HttpGet]
    [Route("{filePath}")]
    public IActionResult ReadFile(string filePath)
    {
        var file = _fileServices.ReadFile(filePath);

        return Ok(file);
    }

    [HttpDelete]
    [Route("{filePath}")]
    public Task<IActionResult> DeleteFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    [Route("{filePath}")]
    public Task<IActionResult> CreateFileAsync(string filePath)
    {
        throw new NotImplementedException();
    }
}
