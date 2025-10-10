using Application.Interface;
using Domain.Entities;
using Infrastructure.Interface;

namespace Application.Services;

public class UserServices : Service<User>, IUserServices
{
    private readonly IRepositoryAsync<User> _repository;

    protected UserServices(IRepositoryAsync<User> repository)
        : base(repository) { }

    public Task<List<User>> GetAllUsers()
    {
        throw new NotImplementedException();
    }

    public Task<User> GetUserByEmail(string email)
    {
        throw new NotImplementedException();
    }
}
