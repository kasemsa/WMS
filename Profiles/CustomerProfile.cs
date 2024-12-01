using AutoMapper;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos.CustomerDtos;

namespace WarehouseManagementSystem.Profiles
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.SalesInvoiceIds, opt => opt.MapFrom(src => src.SalesInvoices.Select(si => si.Id)));

            CreateMap<CreateCustomerDto, Customer>();
            CreateMap<UpdateCustomerDto, Customer>();
        }
    }
}
