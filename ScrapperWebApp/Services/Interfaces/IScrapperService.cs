using ScrapperWebApp.Models;
using ScrapperWebApp.Models.Dtos;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IScrapperService
    {
        Task<List<Empresa>> GetScrappedData(Filtro filtro);
        Task<List<Filtro>> GetMainRecords(Filtro filtro);
        Task<bool> CheckRegistered(List<UraErrorDto> uraErrors);
    }
}
