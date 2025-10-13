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
        List<User> users = await _userServices.GetAllUsers();
        return Ok(users);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest user)
    {
        await _userServices.CreateUser(user);
        await _unitOfWorkAsync.SaveChangesAsync();
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var response = await _userServices.Login(request.Email, request.Password);
        if (!response.Success)
        {
            return Unauthorized(response);
        }
        return Ok(response);
    }
}
