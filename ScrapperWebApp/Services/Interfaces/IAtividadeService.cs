using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IAtividadeService
    {
        Task<ResponseModel> GetAtividadesAsync();
        Task<ResponseModel> CreateAtividadesAsync(List<Atividade> objAtividades);
        Task<ResponseModel> DeleteAllAsync();

    }
}
