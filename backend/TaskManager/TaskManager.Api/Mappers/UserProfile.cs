using AutoMapper;
using TaskManager.Api.Data.Entities;
using TaskManager.Api.DTOs;

namespace TaskManager.Api.Mappers;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
    }
}