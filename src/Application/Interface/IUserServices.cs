using Application.DTOs.Request;
using Application.DTOs.Response;
using Domain.Entities;

namespace Application.Interface;

/// <summary>
/// Agregar cualquier metodo de logica de negocio aqui
/// </summary>
public interface IUserServices : IService<User>
{
    public Task<User> GetUserByEmail(string email);
    public Task<List<User>> GetAllUsers();
    public Task<ResponseDTO> CreateUser(CreateUserRequest request);
    public Task<ResponseDTO> Login(string email, string password);
}
