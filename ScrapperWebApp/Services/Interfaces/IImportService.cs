namespace ScrapperWebApp.Services.Interfaces
{
    public interface IImportService
    {
        public Task<bool> SeedFiltroData();
        public Task<bool> SeedCepData();
        public Task<bool> SeedAtividadeData();

    }
}
