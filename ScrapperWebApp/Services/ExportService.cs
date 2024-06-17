using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ScrapperWebApp.Data;
using ScrapperWebApp.Models;
using ScrapperWebApp.Models.Dtos;
using ScrapperWebApp.Services.Interfaces;
using ScrapperWebApp.Utility;
using System.Runtime.ConstrainedExecution;

namespace ScrapperWebApp.Services
{
    public class ExportService : IExportService
    {
        private readonly IDbContextFactory<ScrapperDbContext> _context;
        private IConfiguration _configurationManager;


        public ExportService(IDbContextFactory<ScrapperDbContext> context, IConfiguration configuration)
        {
            _context = context;
            _configurationManager = configuration;
        }
        public Task<bool> ExportData(List<EmpresaDto> empresas)
        {
            string base64String;
            try
            {
                var ctx = _context.CreateDbContext();
                using (var wb = new XLWorkbook())
                {
                    int maxPhoneCount = empresas.Max(p => p.Telefones.Count);

                    var datatable = Helper.ConvertToDataTableExport(empresas, maxPhoneCount);
                    var sheet = wb.AddWorksheet(datatable, "Export" + DateTime.Now.ToString("ddMMyyyhhmmss"));

                    // Apply font color to columns 1 to 5
                    sheet.Columns(1, 5).Style.Font.FontColor = XLColor.Black;
                    wb.SaveAs(_configurationManager["DirectoryPath"] + "Export" + DateTime.Now.ToString("ddMMyyyhhmmss") + ".xlsx");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }
        public Task<bool> ExportURAData(List<UraErrorDto> uraErrors)
        {
            string base64String;
            try
            {
                var ctx = _context.CreateDbContext();
                using (var wb = new XLWorkbook())
                {
                    var datatable = Helper.ConvertToDataTable(uraErrors);
                    var sheet = wb.AddWorksheet(datatable, "Export_URA_ERRORS" + DateTime.Now.ToString("ddMMyyyhhmmss"));

                    // Apply font color to columns 1 to 5
                    sheet.Columns(1, 5).Style.Font.FontColor = XLColor.Black;
                    wb.SaveAs(_configurationManager["DirectoryPath"] + "Export_URA_ERRORS" + DateTime.Now.ToString("ddMMyyyhhmmss") + ".xlsx");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return Task.FromResult(false);
            }
            return Task.FromResult(true);
        }

        public async Task<ResponseModel> SearchExportData(ExportDto parameters)
        {
            var ctx = _context.CreateDbContext();

            var query = ctx.Empresas.AsQueryable();

            if (parameters.selectedAtividades != null && parameters.selectedAtividades.Any())
            {
                query = query.Where(e => e.EmpAtividades.Any(x => parameters.selectedAtividades.Contains(x.NoAtividade)));
            }
            if (parameters.selectedNatJurs != null && parameters.selectedNatJurs.Any())
            {
                query = query.Where(e => parameters.selectedNatJurs.Contains(e.NoNatjur));
            }
            if (parameters.selectedSitCad != null && !string.IsNullOrEmpty(parameters.selectedSitCad))
            {
                query = query.Where(e => e.CdSituacao == parameters.selectedSitCad);
            }
            if (parameters.selectedEstado != null && parameters.selectedEstado.Any())
            {
                var queryEstados = ctx.Ceps.Where(x => parameters.selectedEstado.Contains(x.CdEstado)).Select(x => x.NoCep).ToList();
                query = query.Where(e => queryEstados.Contains((e.NoCep)));
            }
            if (parameters.selectedCep != null && parameters.selectedCep.Any())
            {
                query = query.Where(e => parameters.selectedCep.Contains((e.NoCep)));
            }
            if (parameters.DateFrom.HasValue)
            {
                query = query.Where(e => e.DtAbertura >= parameters.DateFrom.Value);
            }
            if (parameters.DateUntil.HasValue)
            {
                query = query.Where(e => e.DtAbertura <= parameters.DateUntil.Value);
            }
            if (parameters.somonteMEI == true)
            {
                query = query.Where(e => e.CdMei == "Yes");
            }
            //if (parameters.withPhone == true)
            //{
            //query = query.Where(e => e.Telefones.All(x => Helper.IsCellPhone(x.NoFone)));
            //}
            if (parameters.withoutMEI == true)
            {
                query = query.Where(e => e.CdMei == "No");
            }
            //if (parameters.cellOnly == true)
            //{
            //query = query.Where(e => e.Telefones.All(x => Helper.IsLandline(x.NoFone)));
            //}
            if (parameters.withEmail == true)
            {
                query = query.Where(e => !string.IsNullOrEmpty(e.CdEmail));
            }

            var results = query.Include(x => x.Telefones).Include(x => x.Socios)
            .Join(ctx.Ceps,
                  empresa => empresa.NoCep,
                  cep => cep.NoCep,
                  (empresa, cep) => new
                  {
                      empresa.NoCnpj,
                      empresa.CdRzsocial,
                      empresa.CdFantasia,
                      empresa.CdTipo,
                      empresa.CdSituacao,
                      empresa.DtSituacao,
                      empresa.VlCapsocial,
                      empresa.NoNatjur,
                      empresa.CdMei,
                      empresa.CdLogra,
                      empresa.CdNumero,
                      empresa.NoCep,
                      empresa.NoFone,
                      empresa.CdEmail,
                      empresa.DtAbertura,
                      empresa.Telefones,
                      empresa.Socios,
                      cep.CdEstado,
                  })
            .ToList();

            //var results = await query.Include(x => x.Telefones).Include(x => x.Socios).ToListAsync();

            if (parameters.cellOnly == true)
            {
                results = results.Where(e => e.Telefones.Count() > 0 && e.Telefones.All(x => Helper.IsCellPhone(x.NoFone))).ToList();
            }
            if (parameters.withPhone == true)
            {
                results = results.Where(e => e.Telefones.Count() > 0 && e.Telefones.All(x => Helper.IsLandline(x.NoFone))).ToList();
            }


            var finalresults = results.Select(x => new EmpresaDto
            {
                NoCnpj = x.NoCnpj,
                CdRzsocial = x.CdRzsocial,
                CdFantasia = x.CdFantasia,
                CdTipo = x.CdTipo,
                CdSituacao = x.CdSituacao,
                DtSituacao = x.DtSituacao,
                VlCapsocial = x.VlCapsocial,
                NoNatjur = x.NoNatjur,
                CdMei = x.CdMei,
                CdLogra = x.CdLogra,
                CdNumero = x.CdNumero,
                NoCep = x.NoCep,
                NoFone = x.NoFone,
                CdEmail = x.CdEmail,
                DtAbertura = x.DtAbertura,
                Telefones = x.Telefones,
                Socios = x.Socios,
                CdEstado = x.CdEstado,
            }).ToList();

            return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, finalresults);
        }
    }
}
