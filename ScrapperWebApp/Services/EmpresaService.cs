using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
namespace ScrapperWebApp.Data
{
    public class EmpresaService : IEmpresaService
    {
        private readonly ScrapperDbContext _context;
        private readonly IMapper _mapper;
        public EmpresaService(ScrapperDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<Empresa>> GetEmpresaAsync()
        {
            return await _context.Empresas.ToListAsync();
        }
        public async Task CreateSingleEmpresaAsync(Empresa objEmpresa)
        {
            _context.Empresas.Add(objEmpresa);
            _context.SaveChanges();
        }
        public async Task CreateEmpresaAsync(List<Empresa> objEmpresa)
        {
            foreach (var emp in objEmpresa)
            {
                var emp_from_db = _context.Empresas.Where(x => x.NoCnpj ==  emp.NoCnpj).AsNoTracking().ToList();
                if (emp_from_db != null && emp_from_db.Count == 1)
                {
                    var obj = emp_from_db.FirstOrDefault();
                    obj = _mapper.Map<Empresa>(emp);
                   
                    _context.Empresas.Update(obj);
                    _context.SaveChanges();
                }
                else if(emp_from_db == null || emp_from_db.Count == 0) 
                {
                    _context.Empresas.Add(emp);
                    _context.SaveChanges();
                }
            }
        }
    }
}