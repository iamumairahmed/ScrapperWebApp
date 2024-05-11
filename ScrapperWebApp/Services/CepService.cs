using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Services
{
    public class CepService : ICepService
    {
        private readonly ScrapperDbContext _context;
        public CepService(ScrapperDbContext context)
        {
            _context = context;
        }
        public async Task<List<Cep>> GetCepsAsync()
        {
            return await _context.Ceps.ToListAsync();
        }
        public async Task<bool> CreateCepsAsync(List<Cep> objCeps)
        {

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {

                    //await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Cep ON");
                    await _context.Ceps.AddRangeAsync(objCeps);
                    _context.SaveChanges();
                    //await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Cep OFF");
                    transaction.Commit();


                    //return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    Console.WriteLine(ex.ToString()); return false;
                }
            }
            return true;
        }
        public async Task<bool> DeleteAllAsync()
        {
            await _context.Ceps.ExecuteDeleteAsync();
            return true;
        }
    }
}