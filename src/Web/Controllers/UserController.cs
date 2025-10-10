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
}
