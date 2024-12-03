using AutoMapper;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos.InvoiceDtos;

namespace WarehouseManagementSystem.Profiles
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<InvoiceItemDto, InvoiceItem>();

            CreateMap<SalesInvoice, SalesInvoiceDto>()
                .ForMember(dest => dest.InvoiceItems, opt => opt.MapFrom(src => src.InvoiceItems))
                .ForMember(dest => dest.QRCodeContent, opt => opt.MapFrom(src => $"InvoiceId: {src.Id}"));

            CreateMap<PurchaseInvoice, PurchaseInvoiceDto>().ReverseMap();
        }
    }
}
