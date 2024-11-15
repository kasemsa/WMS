using AutoMapper;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos.InvoiceDtos;

namespace WarehouseManagementSystem
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CreateSelesInvoiceDto, SalesInvoice>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PreviousBalance, opt => opt.Ignore())
                .ForMember(dest => dest.CurrentBalance, opt => opt.Ignore())
                .ForMember(dest => dest.QRCodeContent, opt => opt.Ignore());

            CreateMap<InvoiceItemDto, InvoiceItem>();

            CreateMap<SalesInvoice, SalesInvoiceDto>();
        }
    }
}
