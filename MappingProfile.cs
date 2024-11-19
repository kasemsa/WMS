﻿using AutoMapper;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Dtos.InvoiceDtos;

namespace WarehouseManagementSystem
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<InvoiceItemDto, InvoiceItem>();

            CreateMap<SalesInvoice, SalesInvoiceDto>()
                .ForMember(dest => dest.InvoiceItems, opt => opt.MapFrom(src => src.InvoiceItems))
                .ForMember(dest => dest.QRCodeContent, opt => opt.MapFrom(src => $"InvoiceId: {src.Id}"));


            CreateMap<SalesInvoice, SalesInvoiceDto>();
        }
    }
}
