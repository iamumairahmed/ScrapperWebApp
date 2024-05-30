using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Services
{
    public class NatJurService : INatJurService
    {
        private readonly IDbContextFactory<ScrapperDbContext> _context;

        public NatJurService(IDbContextFactory<ScrapperDbContext> context)
        {
            _context = context;
        }
        public async Task<ResponseModel> GetNatJurAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var natJurs = await ctx.NatJurs.ToListAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, natJurs);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> CreateNatJurAsync(List<NatJur> objNatJurs)
        {
            try
            {
                var ctx = _context.CreateDbContext();
                await ctx.NatJurs.AddRangeAsync(objNatJurs);
                await ctx.SaveChangesAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, objNatJurs);
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
                var deleted = await ctx.NatJurs.ExecuteDeleteAsync();
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