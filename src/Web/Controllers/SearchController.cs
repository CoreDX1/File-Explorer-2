using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class SearchController : ControllerBase
{
    private readonly ILogger<SearchController> _logger;
    private readonly IFileRepository _fileRepository;
    private readonly IFolderRepository _folderRepository;

    public SearchController(
        ILogger<SearchController> logger,
        IFileRepository fileRepository,
        IFolderRepository folderRepository
    )
    {
        _logger = logger;
        _fileRepository = fileRepository;
        _folderRepository = folderRepository;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] string? type,
        [FromQuery] int page = 1,
        [FromQuery] int size = 20
    )
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest("Search query is required");

        _logger.LogInformation("Search query: {Query}, Type: {Type}, Page: {Page}", q, type, page);

        var results = new List<object>();
        var totalResults = 0;

        // Search files if type is not specified or is "file"
        if (string.IsNullOrEmpty(type) || type.Equals("file", StringComparison.OrdinalIgnoreCase))
        {
            var fileResults = await _fileRepository.SearchAsync(q);
            var matchingFiles = fileResults
                .Skip((page - 1) * size)
                .Take(size)
                .Select(f => new
                {
                    Id = f.Id,
                    Name = f.Name,
                    Type = "file",
                    Size = f.Size,
                    ModifiedAt = f.ModifiedAt,
                    Path = f.Path,
                })
                .ToList();

            totalResults += fileResults.Count;
            results.AddRange(matchingFiles);
        }

        // Search folders if type is not specified or is "folder"
        if (string.IsNullOrEmpty(type) || type.Equals("folder", StringComparison.OrdinalIgnoreCase))
        {
            // For folders, we need to search by name in the database
            var folderResults = await SearchFoldersAsync(q, page, size);
            totalResults += folderResults.TotalCount;
            results.AddRange(folderResults.Folders);
        }

        var response = new
        {
            Query = q,
            Type = type,
            Page = page,
            Size = size,
            TotalResults = totalResults,
            Results = results,
        };

        return Ok(response);
    }

    [HttpPost("advanced")]
    public async Task<IActionResult> AdvancedSearch([FromBody] AdvancedSearchRequest request)
    {
        _logger.LogInformation("Advanced search: {Query}", request.Query);

        var query = _fileRepository.Queryable().AsQueryable();

        // Apply filters
        if (!string.IsNullOrWhiteSpace(request.Query))
        {
            query = query.Where(f => f.Name.Contains(request.Query));
        }

        if (!string.IsNullOrWhiteSpace(request.FileType))
        {
            query = query.Where(f => f.ContentType.Contains(request.FileType));
        }

        if (request.CreatedAfter.HasValue)
        {
            query = query.Where(f => f.CreatedAt >= request.CreatedAfter.Value);
        }

        if (request.CreatedBefore.HasValue)
        {
            query = query.Where(f => f.CreatedAt <= request.CreatedBefore.Value);
        }

        if (request.MinSize.HasValue)
        {
            query = query.Where(f => f.Size >= request.MinSize.Value);
        }

        if (request.MaxSize.HasValue)
        {
            query = query.Where(f => f.Size <= request.MaxSize.Value);
        }

        if (request.FolderId.HasValue)
        {
            query = query.Where(f => f.ParentFolderId == request.FolderId.Value);
        }

        var totalResults = await query.CountAsync();
        var results = await query
            .OrderBy(f => f.Name)
            .Skip((request.Page - 1) * request.Size)
            .Take(request.Size)
            .Select(f => new
            {
                Id = f.Id,
                Name = f.Name,
                Type = "file",
                Size = f.Size,
                ContentType = f.ContentType,
                CreatedAt = f.CreatedAt,
                ModifiedAt = f.ModifiedAt,
            })
            .ToListAsync();

        return Ok(
            new
            {
                Query = request,
                TotalResults = totalResults,
                Results = results,
            }
        );
    }

    [HttpGet("suggestions")]
    public async Task<IActionResult> GetSuggestions([FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return Ok(Array.Empty<string>());

        // Get file name suggestions
        var fileSuggestions = await _fileRepository
            .Queryable()
            .Where(f => f.Name.Contains(q))
            .Select(f => f.Name)
            .Take(5)
            .ToListAsync();

        return Ok(fileSuggestions.Distinct().Take(10));
    }

    private async Task<(int TotalCount, object[] Folders)> SearchFoldersAsync(
        string searchTerm,
        int page,
        int size
    )
    {
        // Since we don't have a direct search method for folders in the repository,
        // we'll use the Queryable from the DbContext through the repository pattern
        var folders = await _folderRepository
            .Queryable()
            .Where(f => f.Name.Contains(searchTerm))
            .ToListAsync();

        var pagedFolders = folders
            .Skip((page - 1) * size)
            .Take(size)
            .Select(f => new
            {
                Id = f.Id,
                Name = f.Name,
                Type = "folder",
                CreatedAt = f.CreatedAt,
                ModifiedAt = f.ModifiedAt,
                Path = f.Path,
            })
            .ToArray();

        return (folders.Count, pagedFolders);
    }
}

public record AdvancedSearchRequest(
    string Query,
    string? FileType,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore,
    long? MinSize,
    long? MaxSize,
    Guid? FolderId,
    int Page = 1,
    int Size = 20
);
