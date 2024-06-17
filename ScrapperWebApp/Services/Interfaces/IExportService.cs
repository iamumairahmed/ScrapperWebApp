using ScrapperWebApp.Models;
using ScrapperWebApp.Models.Dtos;

namespace ScrapperWebApp.Services.Interfaces
{
    public interface IExportService
    {
        public Task<ResponseModel> SearchExportData(ExportDto exportDto);
        public Task<bool> ExportData(List<EmpresaDto> Empresa);
        public Task<bool> ExportURAData(List<UraError> UraErrors);
    }
}
