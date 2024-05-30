﻿using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2013.Word;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
using ScrapperWebApp.Utility;

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
        public Task<bool> ExportData(List<Empresa> empresas)
        {
            string base64String;
            var ctx = _context.CreateDbContext();

            //empresas = ctx.Empresas.Take(5).ToList();
            using (var wb = new XLWorkbook())
            {
                int maxPhoneCount = empresas.Max(p => p.Telefones.Count);

                var datatable = Helper.ConvertToDataTableExport(empresas, maxPhoneCount);
                var sheet = wb.AddWorksheet(datatable, "Export"+DateTime.Now.ToString("ddMMyyyhhmmss"));

                // Apply font color to columns 1 to 5
                sheet.Columns(1, 5).Style.Font.FontColor = XLColor.Black;
                wb.SaveAs(_configurationManager["DirectoryPath"] + "Export" + DateTime.Now.ToString("ddMMyyyhhmmss") + ".xlsx");
                
            }
            throw new NotImplementedException();
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
            if (parameters.selectedEstado != null && !string.IsNullOrEmpty(parameters.selectedEstado))
            {
                //query = query.Where(e => e. == parameters.Estado.Value);
            }
            if (parameters.selectedCep != null && parameters.selectedCep.Any())
            {
                query = query.Where(e => parameters.selectedCep.Contains(int.Parse(e.NoCep)));
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
            if (parameters.withPhone == true)
            {
                //query = query.Where(e => e.WithPhone == parameters.WithPhone.Value);
            }
            if (parameters.withoutMEI == true)
            {
                query = query.Where(e => e.CdMei == "No");
            }
            if (parameters.cellOnly == true)
            {
                //query = query.Where(e => e.CellOnly == parameters.CellOnly.Value);
            }
            if (parameters.withEmail == true)
            { 
                query = query.Where(e => !string.IsNullOrEmpty(e.CdEmail));
            }

            var results = await query.ToListAsync();
            return ResponseModel.SuccessResponse(GlobalDeclaration._successResponse, results);
        }
    }
}
