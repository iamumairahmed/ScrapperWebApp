using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface ICepService
    {
        Task<List<Cep>> GetCepsAsync();

        Task<bool> CreateCepsAsync(List<Cep> objCeps);
        Task<bool> DeleteAllAsync();

    }
}
