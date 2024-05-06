using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IAtividadeService
    {
        Task<List<Atividade>> GetAtividadesAsync();
        Task<bool> CreateAtividadesAsync(List<Atividade> objAtividades);
        Task<bool> DeleteAllAsync();

    }
}
