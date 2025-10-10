using Domain.Entities;

namespace Application.Interface;

/// <summary>
/// Agregar cualquier metodo de logica de negocio aqui
/// </summary>
public interface IUserServices : IService<User>
{
    public Task<User> GetUserByEmail(string email);
    public Task<List<User>> GetAllUsers();
}
