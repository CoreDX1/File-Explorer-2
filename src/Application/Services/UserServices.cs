using Application.DTOs.Request;
using Application.DTOs.Response;
using Application.Interface;
using AutoMapper;
using Domain.Entities;
using Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserServices : Service<User>, IUserServices
{
    private readonly IRepositoryAsync<User> _repository;
    private readonly IMapper _mapper;

    public UserServices(IRepositoryAsync<User> repository, IMapper mapper)
        : base(repository)
    {
        _repository = repository;
        _mapper = mapper;
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
        var user = _mapper.Map<User>(request);

        Insert(user);

        var response = new ResponseDTO()
        {
            Data = user,
            Message = "User created successfully",
            Success = true,
        };

        return response;
    }
}
