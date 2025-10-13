using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using Domain.Entities;
using Infrastructure.Interface;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserServices : Service<User>, IUserServices
{
    private readonly IRepositoryAsync<User> _repository;

    public UserServices(IRepositoryAsync<User> repository)
        : base(repository)
    {
        _repository = repository;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await Queryable().ToListAsync();
    }

    public async Task<User> GetUserByEmail(string email)
    {
        return await Queryable().FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ResponseDTO> CreateUser(CreateUserRequest request)
    {
        var user = request.Adapt<User>();
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        Insert(user);

        var response = new ResponseDTO
        {
            Data = user,
            Message = "User created successfully",
            Success = true,
        };

        return response;
    }
}
