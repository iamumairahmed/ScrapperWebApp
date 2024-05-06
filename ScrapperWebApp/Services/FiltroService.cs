using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Services
{
    public class FiltroService : IFiltroService
    {
        private readonly ScrapperDbContext _context;
        public FiltroService(ScrapperDbContext context)
        {
            _context = context;
        }
        public async Task<Filtro> GetFirstFiltrosAsync()
        {
            return await _context.Filtros.FirstOrDefaultAsync();
        }
        public async Task<List<Filtro>> GetFiltrosAsync()
        {
            return await _context.Filtros.ToListAsync();
        }
        public async Task<List<Filtro>> GetFiltrosWhereAsync()
        {
            return await _context.Filtros.Where(x => x.NoContador > 1000 && x.DtInicial < DateTime.Now).ToListAsync();
        }
        public Task<Filtro> CreateFiltroAsync(Filtro objFiltro)
        {
            _context.Filtros.AddAsync(objFiltro);
            _context.SaveChanges();
            return Task.FromResult(objFiltro);
        }
        public Task<Filtro> UpdateFiltroAsync(Filtro objFiltro)
        {
            var filtro = _context.Filtros.Where(f => f.NoCep == objFiltro.NoCep && f.DtInicial == objFiltro.DtInicial && f.DtFinal == objFiltro.DtFinal).FirstOrDefault();
            if (filtro == null) 
            {
                return null;
            }
            filtro.NoContador = objFiltro.NoContador;
            filtro.DtExecucao = objFiltro.DtExecucao;

            _context.Filtros.Update(filtro);
            _context.SaveChanges();
            return Task.FromResult(filtro);
        }
        public async Task<bool> CreateFiltrosAsync(List<Filtro> objFiltros)
        {
            try
            {
                await _context.Filtros.AddRangeAsync(objFiltros);
                _context.SaveChanges();

                //using (var transaction = _context.Database.BeginTransaction())
                //{

                //    //await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Filtro ON");
                //    await _context.Filtros.AddRangeAsync(objFiltros);
                //    _context.SaveChanges();
                //    //await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Filtro OFF");
                //    transaction.Commit();
                //}

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString()); return false;
            }
        }
        public async Task<bool> DeleteAllAsync() 
        {
            await _context.Filtros.ExecuteDeleteAsync();
            return true;
        }
    }
}