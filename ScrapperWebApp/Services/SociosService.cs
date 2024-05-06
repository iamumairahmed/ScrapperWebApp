using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
namespace ScrapperWebApp.Data
{
    public class SocioService
    {
        private readonly ScrapperDbContext _context;
        public SocioService(ScrapperDbContext context)
        {
            _context = context;
        }
        public async Task<List<Socio>> GetSociosAsync()
        {
            return await _context.Socios.ToListAsync();
        }
    }
}