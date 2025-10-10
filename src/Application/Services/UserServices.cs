using System.Linq;
using Application.Interface;
using Domain.Entities;
using Infrastructure.Interface;
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
}
