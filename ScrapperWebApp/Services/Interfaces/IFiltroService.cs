using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IFiltroService
    {
        Task<Filtro> GetFirstFiltrosAsync();

        Task<List<Filtro>> GetFiltrosAsync();
        Task<List<Filtro>> GetFiltrosWhereAsync();

        Task<Filtro> CreateFiltroAsync(Filtro objFiltro);

        Task<Filtro> UpdateFiltroAsync(Filtro objFiltro);

        Task<bool> CreateFiltrosAsync(List<Filtro> objFiltros);
        Task<bool> DeleteAllAsync();
    }
}
