using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Services
{
    public class FiltroService : IFiltroService
    {
        private readonly IDbContextFactory<ScrapperDbContext> _context;
        private readonly IMapper _mapper;
        public FiltroService(IDbContextFactory<ScrapperDbContext> context)
        {
            _context = context;
        }
        public async Task<ResponseModel> GetFirstFiltrosAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();

                var filtro = await ctx.Filtros.FirstOrDefaultAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, filtro);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }

        public async Task<ResponseModel> GetFiltrosAsync()
        {
            try
            { 
                var ctx = _context.CreateDbContext();
                var filtros = await ctx.Filtros.ToListAsync();
                //var appSettingsVm = _mapper.Map<List<AppSettingVm>>(appsettigs);
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, filtros);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }

        public async Task<ResponseModel> GetFiltrosWhereAsync()
        {
            try
            {
                var ctx = _context.CreateDbContext();

                //var filtros = _unitOfWork.Repository<Filtro>().GetMany(x => x.NoContador > 1000 && x.DtInicial < DateTime.Now).ToList();
                var filtros = ctx.Filtros.ToList();
                if (filtros != null)
                {
                    //var filtrosMapped = _mapper.Map<Filtro>(filtros);
                    return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, filtros);
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
        public async Task<ResponseModel> CreateFiltroAsync(Filtro objFiltro)
        {
            //var filtro = mapper.Map<AppSetting>(appSettingVm);
            var ctx = _context.CreateDbContext();

            try
            {
                await ctx.AddAsync(objFiltro);
                await ctx.SaveChangesAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, objFiltro);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> UpdateFiltroAsync(Filtro objFiltro)
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var filtroFromDb = ctx.Filtros.Where(f => f.NoCep == objFiltro.NoCep && f.DtInicial == objFiltro.DtInicial && f.DtFinal == objFiltro.DtFinal).FirstOrDefault();
                if (filtroFromDb == null)
                {
                    return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, filtroFromDb);
                }
                else
                {
                    filtroFromDb.NoContador = objFiltro.NoContador;
                    filtroFromDb.DtExecucao = objFiltro.DtExecucao;
                    ctx.Update(filtroFromDb);
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
        public async Task<ResponseModel> UpdateFiltroDatesAsync(Filtro objFiltro)
        {
            try
            {
                var ctx = _context.CreateDbContext();
                var filtroFromDb = ctx.Filtros.Where(f => f.NoCep == objFiltro.NoCep && f.DtInicial == objFiltro.DtInicial && f.DtFinal == objFiltro.DtFinal).FirstOrDefault();
                if (filtroFromDb == null)
                {
                    return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, filtroFromDb);
                }
                else
                {
                    var response = await DeleteAsync(filtroFromDb);
                    if (response.Success)
                    {
                        filtroFromDb.DtInicial = filtroFromDb.DtInicial.AddDays(1);
                        filtroFromDb.DtFinal = filtroFromDb.DtFinal.AddDays(1);
                        await CreateFiltroAsync(filtroFromDb);
                        
                        //_unitOfWork.Repository<Filtro>().Update(filtroFromDb);
                        //await _unitOfWork.SaveAsync();
                        return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, true);
                    }
                    else return ResponseModel.SuccessResponse("Could not update Filter", false);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> CreateFiltrosAsync(List<Filtro> objFiltros)
        {

            try
            {
                var ctx = _context.CreateDbContext();

                await ctx.Filtros.AddRangeAsync(objFiltros);
                await ctx.SaveChangesAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, objFiltros);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }


            //try
            //{
            //    await _context.Filtros.AddRangeAsync(objFiltros);
            //    _context.SaveChanges();

            //    //using (var transaction = _context.Database.BeginTransaction())
            //    //{

            //    //    //await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Filtro ON");
            //    //    await _context.Filtros.AddRangeAsync(objFiltros);
            //    //    _context.SaveChanges();
            //    //    //await _context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT dbo.Filtro OFF");
            //    //    transaction.Commit();
            //    //}

            //    return true;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.ToString()); return false;
            //}
        }
        public async Task<ResponseModel> DeleteAllAsync() 
        {
            try
            {
                var ctx = _context.CreateDbContext();

                var deleted = await ctx.Filtros.ExecuteDeleteAsync();
                //if (deleted == true)
                //{
                 //   await _unitOfWork.SaveAsync();
                    return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, true);
                //}
                //else
                //    return ResponseModel.FailureResponse("Not Found");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }

            //await _context.Filtros.ExecuteDeleteAsync();
            //return true;
        }
        public async Task<ResponseModel> DeleteAsync(Filtro objFiltro)
        {
            try
            {
                var ctx = _context.CreateDbContext();

                var filtroFromDb = ctx.Filtros.Where(f => f.NoCep == objFiltro.NoCep && f.DtInicial == objFiltro.DtInicial && f.DtFinal == objFiltro.DtFinal).FirstOrDefault();
                if (filtroFromDb != null)
                {
                    ctx.Filtros.Remove(filtroFromDb);
                    await ctx.SaveChangesAsync();
                    //var deleted = await _unitOfWork.Repository<Filtro>().Delete(filtroFromDb);
                    //if (deleted == true)
                    //{
                    //    await _unitOfWork.SaveAsync();
                        return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, null);
                    //}
                    //else 
                    //{
                    //    return ResponseModel.FailureResponse("Could not delete");
                   // }
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