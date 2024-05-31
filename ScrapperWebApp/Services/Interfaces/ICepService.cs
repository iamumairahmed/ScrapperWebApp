using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface ICepService
    {
        Task<ResponseModel> GetCepsAsync();
        Task<ResponseModel> GetSingleCepAsync(string value);
        Task<ResponseModel> GetEstadoAsync();
        Task<ResponseModel> CreateCepsAsync(List<Cep> objCeps);
        Task<ResponseModel> DeleteAllAsync();

    }
}
