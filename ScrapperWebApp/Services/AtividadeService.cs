using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Data
{
    public class AtividadeService : IAtividadeService
    {
        private readonly ScrapperDbContext _context;
        public AtividadeService(ScrapperDbContext context)
        {
            _context = context;
        }
        public async Task<List<Atividade>> GetAtividadesAsync()
        {
            return await _context.Atividades.ToListAsync();
        }
        public async Task<bool> CreateAtividadesAsync(List<Atividade> objAtividade)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Atividade ON");
                    await _context.Atividades.AddRangeAsync(objAtividade);
                    _context.SaveChanges();
                    await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Atividade OFF");
                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.ToString()); return false;
                }
            }
        }
        public async Task<bool> DeleteAllAsync()
        {
            await _context.Atividades.ExecuteDeleteAsync();
            return true;
        }
    }
}