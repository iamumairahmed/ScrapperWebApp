using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Data
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IDbContextFactory<ScrapperDbContext> _context;
        private readonly IMapper _mapper;
        public EmpresaService(IDbContextFactory<ScrapperDbContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<Empresa>> GetEmpresaAsync()
        {
            var ctx = _context.CreateDbContext();
            return await ctx.Empresas.ToListAsync();
        }
        public async Task<ResponseModel> CreateFiltroAsync(Empresa objEmpresa)
        {
            try
            {
                var ctx = _context.CreateDbContext();

                await ctx.Empresas.AddAsync(objEmpresa);
                await ctx.SaveChangesAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, objEmpresa);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> CreateSingleEmpresaAsync(Empresa objEmpresa)
        {
            try
            {
                var ctx = _context.CreateDbContext();

                await ctx.Empresas.AddAsync(objEmpresa);
                await ctx.SaveChangesAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, objEmpresa);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> CreateEmpresaAsync(List<Empresa> objEmpresa)
        {
            try
            {
                var ctx = _context.CreateDbContext();

                foreach (var emp in objEmpresa)
                {
                    var emp_from_db = await ctx.Empresas.AsNoTracking().Where(x => x.NoCnpj == emp.NoCnpj).FirstOrDefaultAsync();

                    if (emp_from_db != null)
                    {
                        var obj = _mapper.Map<Empresa>(emp);

                        ctx.Empresas.Update(obj);
                        await ctx.SaveChangesAsync();

                    }
                    else if (emp_from_db == null)
                    {
                        await ctx.Empresas.AddAsync(emp);
                        await ctx.SaveChangesAsync();
                    }
                }
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
        public async Task<ResponseModel> DeleteAsync(Empresa objEmpresa)
        {
            try
            {
                var ctx = _context.CreateDbContext();

                ctx.Empresas.Remove(objEmpresa);
                await ctx.SaveChangesAsync();
                return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return ResponseModel.FailureResponse(GlobalDeclaration._internalServerError);
            }
        }
    }
}