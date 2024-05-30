using ScrapperWebApp.Models;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IExportService
    {
        public Task<ResponseModel> SearchExportData(ExportDto exportDto);
        public Task<bool> ExportData(List<Empresa> Empresa);
    }
}
