using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IURAService
    {
        Task<ResponseModel> GetURAAsync();
        Task<ResponseModel> GetRegistrosCountAsync();
        Task<ResponseModel> GetForAnalysisCountAsync();
        Task<ResponseModel> CreateURAAsync(List<UraError> objUras);
        Task<ResponseModel> DeleteAllAsync();
        Task<ResponseModel> UpdateURAAsync(UraError uraError);
    }
}
