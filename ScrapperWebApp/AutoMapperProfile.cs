using AutoMapper;
using ScrapperWebApp.Models;
using System;

namespace ScrapperWebApp
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Empresa, Empresa>();
        }
    }
}
