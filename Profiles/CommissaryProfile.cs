using AutoMapper;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos.CommissaryDtos;

namespace WarehouseManagementSystem.Profiles
{
    public class CommissaryProfile : Profile
    {
        public CommissaryProfile()
        {
            // Mapping for creating a Commissary from CreateCommissaryDto
            CreateMap<CreateCommissaryDto, Commissary>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore()); // UserId is set separately after User creation

            // Mapping for creating a User from CreateCommissaryDto
            CreateMap<CreateCommissaryDto, User>()
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            // Mapping for updating an existing Commissary with UpdateCommissaryDto
            CreateMap<UpdateCommissaryDto, Commissary>()
                .ForMember(dest => dest.Name, opt => opt.Condition(src => src.Name != null))
                .ForMember(dest => dest.PhoneNumber, opt => opt.Condition(src => src.PhoneNumber != null));


            // Mapping Commissary to CommissaryDto for read-only data retrieval
            CreateMap<Commissary, CommissaryDto>();
        }
    }
}