using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IEmpresaService
    {
        Task<List<Empresa>> GetEmpresaAsync();
        Task<ResponseModel> CreateFiltroAsync(Empresa objEmpresa);
        Task<ResponseModel> CreateSingleEmpresaAsync(Empresa objEmpresa);
        Task<ResponseModel> CreateEmpresaAsync(List<Empresa> objEmpresa);
        Task<ResponseModel> DeleteAsync(Empresa objEmpresa);
    }
}
