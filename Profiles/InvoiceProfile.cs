using AutoMapper;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos.InvoiceDtos;

namespace WarehouseManagementSystem.Profiles
{
    public class InvoiceProfile : Profile
    {
        public InvoiceProfile()
        {
            CreateMap<SalesInvoice, SalesInvoiceDto>()
    .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
    .ForMember(dest => dest.CommissaryName, opt => opt.MapFrom(src => src.Commissary.Name))
    .ForMember(dest => dest.InvoiceItems, opt => opt.MapFrom(src => src.InvoiceItems))
    .ForMember(dest => dest.QRCodeContent, opt => opt.MapFrom(src => $"InvoiceId: {src.Id}"));

            CreateMap<SalesInvoiceDto, SalesInvoice>()
                .ForMember(dest => dest.Customer, opt => opt.Ignore())
                .ForMember(dest => dest.Commissary, opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceItems, opt => opt.MapFrom(src => src.InvoiceItems));


            CreateMap<PurchaseInvoice, PurchaseInvoiceDto>().ReverseMap();


            CreateMap<PurchaseInvoice, PurchaseInvoiceDto>()
                .ForMember(dest => dest.CommissaryName, opt => opt.MapFrom(src => src.Commissary.Name));

            CreateMap<PurchaseInvoiceDto, PurchaseInvoice>()
                .ForMember(dest => dest.Commissary, opt => opt.Ignore());


            CreateMap<InvoiceItem, InvoiceItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

            CreateMap<InvoiceItemDto, InvoiceItem>()
                .ForMember(dest => dest.Product, opt => opt.Ignore())
                .ForMember(dest => dest.SalesInvoice, opt => opt.Ignore())
                .ForMember(dest => dest.PurchaseInvoice, opt => opt.Ignore());
        }
    }
}
