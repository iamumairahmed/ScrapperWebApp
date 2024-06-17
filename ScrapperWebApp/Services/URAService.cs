using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Services
{
    public class URAService : IURAService
    {
        private readonly IDbContextFactory<ScrapperDbContext> _context;

        public URAService(IDbContextFactory<ScrapperDbContext> context)
        {
            _context = context;
        }
        public async Task<ResponseModel> GetURAAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var uraErrors = await ctx.UraErrors.ToListAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, uraErrors);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> GetRegistrosCountAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();
                int count = await ctx.UraErrors.CountAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> GetForAnalysisCountAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var analysisCount = await ctx.UraErrors.Where(u => u.CdErrors != null).CountAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, analysisCount);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> CreateURAAsync(List<UraError> objUraErrors)
        {
            try
            {
                var ctx = _context.CreateDbContext();
                await ctx.UraErrors.AddRangeAsync(objUraErrors);
                await ctx.SaveChangesAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, objUraErrors);
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
                var deleted = await ctx.UraErrors.ExecuteDeleteAsync();
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
        public async Task<ResponseModel> UpdateURAAsync(UraError uraError)
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var uraErrorFromDb = ctx.UraErrors.Where(f => f.NoUraErr == uraError.NoUraErr).FirstOrDefault();
                if (uraErrorFromDb == null)
                {
                    return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, uraErrorFromDb);
                }
                else
                {
                    uraErrorFromDb.CdErrors = uraError.CdErrors;
                    
                    ctx.Update(uraErrorFromDb);
                    await ctx.SaveChangesAsync();
                    return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, true);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
    }
}