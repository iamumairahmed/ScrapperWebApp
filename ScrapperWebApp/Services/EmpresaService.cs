using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
using System.Collections.Generic;
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

                var distinctList = objEmpresa.Distinct(new EmpresaEqualityComparer()).ToList();

                foreach (var emp in distinctList)
                {
                    Console.WriteLine(DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + ": Saving " + emp.NoCnpj);

                    var emp_from_db = await ctx.Empresas.AsNoTracking().Where(x => x.NoCnpj == emp.NoCnpj)
                        .Include(x => x.EmpAtividades)
                        .Include(x => x.Socios)
                        .Include(x => x.Telefones)
                        .FirstOrDefaultAsync();

                    if (emp_from_db != null)
                    {
                        await DeleteAsync(emp_from_db);

                        await ctx.Empresas.AddAsync(emp);
                        await ctx.SaveChangesAsync();

                        //    var obj = _mapper.Map<Empresa>(emp);

                        //    //emp_from_db.EmpAtividades.Remove(obj.EmpAtividades);

                        //    ctx.Empresas.Update(emp_from_db);
                        //    ctx.Entry(emp_from_db).State = EntityState.Modified;
                        //    ctx.Entry(emp_from_db.Socios.GetType).State = EntityState.Modified;
                        //    ctx.Entry(emp_from_db.EmpAtividades.GetType).State = EntityState.Modified;
                        //    ctx.Entry(emp_from_db.Telefones.GetType).State = EntityState.Modified;

                        //    await ctx.SaveChangesAsync();

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
        public class EmpresaEqualityComparer : IEqualityComparer<Empresa>
        {
            public bool Equals(Empresa x, Empresa y)
            {
                if (x == null || y == null) return false;
                return x.NoCnpj == y.NoCnpj && x.CdRzsocial == y.CdRzsocial;
            }

            public int GetHashCode(Empresa obj)
            {
                return HashCode.Combine(obj.NoCnpj, obj.CdRzsocial);
            }
        }
    }
}