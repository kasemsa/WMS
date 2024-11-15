using AutoMapper;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos.UserDtos;

namespace WarehouseManagementSystem.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<User, UpdateUserDto>().ReverseMap();
        }
    }
}
