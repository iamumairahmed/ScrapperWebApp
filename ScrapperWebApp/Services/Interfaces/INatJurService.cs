using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface INatJurService
    {
        Task<ResponseModel> GetNatJurAsync();

        Task<ResponseModel> CreateNatJurAsync(List<NatJur> objNatJurs);
        Task<ResponseModel> DeleteAllAsync();

    }
}
