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
            CreateMap<UraError, Person>()
             .ForMember(dest => dest.NoUraErr, opt => opt.MapFrom(src => src.NoUraErr))
             .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => src.NoFone1))
             .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.CdEmail))
             .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.NoCnpj.HasValue ? src.NoCnpj.Value.ToString() : string.Empty))
             .ForMember(dest => dest.Razao, opt => opt.MapFrom(src => src.CdRzsocial))
             //.ForMember(dest => dest.Errors, opt => opt.MapFrom(src => src.CdErrors.Split(',').ToList()))
             .ForMember(dest => dest.Firstname, opt => opt.MapFrom<FirstnameResolver>())
             .ForMember(dest => dest.Lastname, opt => opt.MapFrom<LastnameResolver>());
        }
    }

    public class LastnameResolver : IValueResolver<UraError, Person, string>
    {
        public string Resolve(UraError source, Person destination, string destMember, ResolutionContext context)
        {
            var nameTokens = source.DsSocio?.Split(' ') ?? new string[0];
            return nameTokens.Length > 1 ? string.Join(" ", nameTokens.Skip(1)) : string.Empty;
        }
    }
    public class FirstnameResolver : IValueResolver<UraError, Person, string>
    {
        public string Resolve(UraError source, Person destination, string destMember, ResolutionContext context)
        {
            var nameTokens = source.DsSocio?.Split(' ') ?? new string[0];
            return nameTokens.Length > 0 ? nameTokens[0] : string.Empty;
        }
    }
}
