using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
using ScrapperWebApp.UnitOfWork;
namespace ScrapperWebApp.Services
{
    public class CepService : ICepService
    {
        private readonly ScrapperDbContext _context;
        private readonly IUnitOfWork _unitOfWork;

        public CepService(ScrapperDbContext context, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public async Task<ResponseModel> GetCepsAsync()
        {
            try
            {
                var ceps = await _unitOfWork.Repository<Cep>()
               .TableNoTracking
               .ToListAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, ceps);
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
                await _unitOfWork.Repository<Cep>().AddRangeAsync(objCeps);
                await _unitOfWork.SaveAsync();
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
                var deleted = await _unitOfWork.Repository<Cep>().Delete(x => 1 == 1);
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