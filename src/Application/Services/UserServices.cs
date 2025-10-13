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
    public UserServices(IRepositoryAsync<User> repository)
        : base(repository) { }

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

    public async Task<ResponseDTO> Login(string email, string password)
    {
        var user = await GetUserByEmail(email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            return new ResponseDTO
            {
                Data = null,
                Message = "Invalid email or password",
                Success = false,
            };
        }

        user.LastLoginAt = DateTime.UtcNow;
        Update(user);

        return new ResponseDTO
        {
            Data = user,
            Message = "Login successful",
            Success = true,
        };
    }

    public Task<AuthResult> AuthenticateAsync(string email, string password)
    {
        throw new NotImplementedException();
    }

    public async Task<AuthResult> CreateUserAsync(CreateUserRequest request)
    {
        var user = await GetUserByEmail(request.Email);
        if (user != null)
            return new AuthResult(false, "User already exists");

        var newUser = request.Adapt<User>();
        newUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        Insert(newUser);
        return new AuthResult(true, "User created successfully");
    }

    public Task<AuthResult> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task RevokeTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<AuthResult> GoogleAuthAsync(string idToken)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetAsync(string email)
    {
        throw new NotImplementedException();
    }
}
