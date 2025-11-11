using Application.DTOs.Request;
using Application.Interface;
using Domain.Entities;
using Infrastructure.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserServices _userServices;
    private readonly IUnitOfWorkAsync _unitOfWorkAsync;

    public UserController(IUserServices userServices, IUnitOfWorkAsync unitOfWorkAsync)
    {
        _userServices = userServices;
        _unitOfWorkAsync = unitOfWorkAsync;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userServices.GetAllUsersAsync();

        if (result.Metadata?.StatusCode != 200)
        {
            return StatusCode(
                result.Metadata?.StatusCode ?? 500,
                new { message = result.Metadata?.Message }
            );
        }

        return Ok(result);
    }

    // [HttpPost]
    // public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest user)
    // {
    //     await _userServices.CreateUser(user);
    //     await _unitOfWorkAsync.SaveChangesAsync();
    //     return Ok(user);
    // }

    // [HttpPost("login")]
    // public async Task<IActionResult> Login([FromBody] LoginRequest request)
    // {
    //     var response = await _userServices.Login(request.Email, request.Password);
    //     if (!response.Metadata.StatusCode.Equals(200))
    //     {
    //         return Unauthorized(response);
    //     }
    //     return Ok(response);
    // }
}
