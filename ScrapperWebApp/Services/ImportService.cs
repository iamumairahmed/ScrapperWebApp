using ExcelDataReader;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
using System.Data;

namespace ScrapperWebApp.Services
{
    public class ImportService : IImportService
    {
        private IFiltroService _filtroService;
        private ICepService _cepService;
        private IAtividadeService _atividadeService;
        public ImportService(IFiltroService filtroService, IAtividadeService atividadeService, ICepService cepService)
        {
            _filtroService = filtroService;
            _atividadeService = atividadeService;
            _cepService = cepService;
        }

        public async Task<bool> SeedFiltroData(){
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            List<Filtro> list = new List<Filtro>();
            try
            {
                var filepath = @"D:\Fiverr\SamuelRoncetti\Filtro Short.xlsm";
                using (var streamval = File.Open(filepath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(streamval))
                    {
                        var configuration = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = false
                            }
                        };
                        var dataSet = reader.AsDataSet(configuration);

                        if (dataSet.Tables.Count > 0)
                        {
                            var dataTable = dataSet.Tables[0];
                            Console.WriteLine("Rows : " + dataTable.Rows.Count);
                            Console.WriteLine("Columns : " + dataTable.Columns.Count);
                            foreach (DataRow row in dataTable.Rows)
                            {
                                Filtro obj = new Filtro();
                                string value = row[0].ToString();
                                string date_start = row[1].ToString();
                                string date_end = row[2].ToString();

                                obj.NoCep = value;
                                obj.DtInicial = DateTime.Parse(date_start);
                                obj.DtFinal = DateTime.Parse(date_end);
                                list.Add(obj);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Sheet doesn't exist");
                            return false;

                        }

                        if (list.Count > 0)
                        {
                            await _filtroService.DeleteAllAsync();
                            await _filtroService.CreateFiltrosAsync(list);
                        }
                        else
                        {
                            Console.WriteLine("No Data Found!");
                            return false;

                        }

                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        public async Task<bool> SeedAtividadeData()
        {
            try
            {
                var filepath = @"D:\Fiverr\SamuelRoncetti\Atividade (2).xlsx";
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                List<Atividade> list = new List<Atividade>();
                using (var streamval = File.Open(filepath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(streamval))
                    {
                        var configuration = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = false
                            }
                        };
                        var dataSet = reader.AsDataSet(configuration);

                        if (dataSet.Tables.Count > 0)
                        {
                            var dataTable = dataSet.Tables[0];
                            Console.WriteLine("Rows : " + dataTable.Rows.Count);
                            Console.WriteLine("Columns : " + dataTable.Columns.Count);
                            foreach (DataRow row in dataTable.Rows)
                            {
                                Atividade obj = new Atividade();
                                string noAtividade = row[0].ToString();
                                string dsAtividade = row[1].ToString();

                                obj.NoAtividade = int.Parse(noAtividade);
                                obj.DsAtividade = dsAtividade;
                                list.Add(obj);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Sheet doesn't exist");
                            return false;
                        }

                        if (list.Count > 0)
                        {
                            await _atividadeService.DeleteAllAsync();
                            await _atividadeService.CreateAtividadesAsync(list);
                        }
                        else
                        {
                            Console.WriteLine("No Data Found!");
                            return false;
                        }
                    }
                }
            } catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        public async Task<bool> SeedCepData()
        {
            try
            {
                var filepath = @"D:\Fiverr\SamuelRoncetti\Cep r02.xlsm";
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                List<Cep> list = new List<Cep>();
                using (var streamval = File.Open(filepath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(streamval))
                    {
                        var configuration = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = false
                            }
                        };
                        var dataSet = reader.AsDataSet(configuration);

                        if (dataSet.Tables.Count > 0)
                        {
                            var dataTable = dataSet.Tables[0];
                            Console.WriteLine("Rows : " + dataTable.Rows.Count);
                            Console.WriteLine("Columns : " + dataTable.Columns.Count);
                            foreach (DataRow row in dataTable.Rows)
                            {
                                Cep obj = new Cep();
                                string nocep = row[0].ToString();
                                string cdLogradouro = row[1].ToString();
                                string cdBairro = row[2].ToString();
                                string cdMunicipio = row[3].ToString();
                                string cdEstado = row[4].ToString();

                                obj.NoCep = nocep;
                                obj.CdLogradouro = cdLogradouro;
                                obj.CdBairro = cdBairro;
                                obj.CdMunicipio = cdMunicipio;
                                obj.CdEstado = cdEstado;

                                list.Add(obj);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Sheet doesn't exist");
                            return false;
                        }

                        if (list.Count > 0)
                        {
                            await _cepService.DeleteAllAsync();
                            await _cepService.CreateCepsAsync(list);
                        }
                        else
                        {
                            Console.WriteLine("No Data Found!");
                            return false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
    }
}
