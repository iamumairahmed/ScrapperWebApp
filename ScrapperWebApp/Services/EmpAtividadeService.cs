using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
namespace ScrapperWebApp.Data
{
    public class EmpAtividadeService
    {
        private readonly ScrapperDbContext _context;
        public EmpAtividadeService(ScrapperDbContext context)
        {
            _context = context;
        }
        public async Task<List<EmpAtividade>> GetEmpAtividadesAsync()
        {
            return await _context.EmpAtividades.ToListAsync();
        }
    }
}