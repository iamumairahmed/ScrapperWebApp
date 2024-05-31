using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Services
{
    public class CepService : ICepService
    {
        private readonly IDbContextFactory<ScrapperDbContext> _context;

        public CepService(IDbContextFactory<ScrapperDbContext> context)
        {
            _context = context;
        }
        public async Task<ResponseModel> GetCepsAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var ceps = await ctx.Ceps.ToListAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, ceps);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> GetSingleCepAsync(string value)
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var ceps = await ctx.Ceps.Where(x => x.NoCep == value).ToListAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, ceps);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> GetEstadoAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var estados = ctx.Ceps.Select(u => u.CdEstado).Distinct().ToList();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, estados);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> CreateCepsAsync(List<Cep> objCeps)
        {
            try
            {
                var ctx = _context.CreateDbContext();
                await ctx.Ceps.AddRangeAsync(objCeps);
                await ctx.SaveChangesAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, objCeps);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> DeleteAllAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var deleted = await ctx.Ceps.ExecuteDeleteAsync();
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