using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
using ScrapperWebApp.UnitOfWork;
namespace ScrapperWebApp.Data
{
    public class AtividadeService : IAtividadeService
    {
        private readonly ScrapperDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public AtividadeService(ScrapperDbContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }
        public async Task<ResponseModel> GetAtividadesAsync()
        {
            try
            {
                var filtros = await _unitOfWork.Repository<Atividade>()
               .TableNoTracking
               .ToListAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, filtros);
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
                await _unitOfWork.Repository<Atividade>().AddRangeAsync(objAtividade);
                await _unitOfWork.SaveAsync();
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
                var deleted = await _unitOfWork.Repository<Atividade>().Delete(x => 1 == 1);
                if (deleted == true)
                {
                    await _unitOfWork.SaveAsync();
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