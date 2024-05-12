using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Data
{
    public class AtividadeService : IAtividadeService
    {
        private readonly IDbContextFactory<ScrapperDbContext> _context;
        public AtividadeService(IDbContextFactory<ScrapperDbContext> context)
        {
            _context = context;
        }
        public async Task<ResponseModel> GetAtividadesAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var atividades = await ctx.Atividades.ToListAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, atividades);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> CreateAtividadesAsync(List<Atividade> objAtividade)
        {
            try
            {
                var ctx = _context.CreateDbContext();
                await ctx.Atividades.AddRangeAsync(objAtividade);
                await ctx.SaveChangesAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, objAtividade);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
            //using (var transaction = _context.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Atividade ON");
            //        await _context.Atividades.AddRangeAsync(objAtividade);
            //        _context.SaveChanges();
            //        await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Atividade OFF");
            //        transaction.Commit();

            //        return true;
            //    }
            //    catch (Exception ex)
            //    {
            //        transaction.Rollback();
            //        Console.WriteLine(ex.ToString()); return false;
            //    }
            //}
        }
        public async Task<ResponseModel> DeleteAllAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var deleted = await ctx.Atividades.ExecuteDeleteAsync();
                if (deleted > 0)
                {
                    await ctx.SaveChangesAsync();
                    return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, true);
                }
                else
                    return ResponseModel.FailureResponse("Not Found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
    }
}