using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IFiltroService
    {
        Task<ResponseModel> GetFirstFiltrosAsync();

        Task<ResponseModel> GetFiltrosAsync();
        Task<ResponseModel> GetFiltrosWhereAsync();

        Task<ResponseModel> CreateFiltroAsync(Filtro objFiltro);

        Task<ResponseModel> UpdateFiltroAsync(Filtro objFiltro);
        Task<ResponseModel> UpdateFiltroDatesAsync(Filtro objFiltro);

        Task<ResponseModel> CreateFiltrosAsync(List<Filtro> objFiltros);
        Task<ResponseModel> DeleteAllAsync();
    }
}
