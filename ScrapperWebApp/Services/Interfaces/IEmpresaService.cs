using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IEmpresaService
    {
        Task<List<Empresa>> GetEmpresaAsync();
        Task CreateSingleEmpresaAsync(Empresa objEmpresa);
        Task CreateEmpresaAsync(List<Empresa> objEmpresa);
        
    }
}
