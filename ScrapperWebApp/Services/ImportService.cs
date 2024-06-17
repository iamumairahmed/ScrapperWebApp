using ExcelDataReader;
using ScrapperWebApp.Models;
using ScrapperWebApp.Services.Interfaces;
using System.Data;

namespace ScrapperWebApp.Services
{
    public class ImportService : IImportService
    {
        private IFiltroService _filtroService;
        private IURAService _uraService;
        private ICepService _cepService;
        private IAtividadeService _atividadeService;
        private IConfiguration _configurationManager;
        public ImportService(IFiltroService filtroService, IAtividadeService atividadeService, ICepService cepService, IConfiguration configurationManager, IURAService uraService)
        {
            _filtroService = filtroService;
            _atividadeService = atividadeService;
            _cepService = cepService;
            _configurationManager = configurationManager;
            _uraService = uraService;
        }

        public async Task<bool> SeedFiltroData(){
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            List<Filtro> list = new List<Filtro>();
            try
            {
                var filepath = _configurationManager["DirectoryPath"] + "Filtro Short.xlsm";
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
                            foreach (DataRow row in dataTable.Rows)
                            {
                                if (row[0].ToString() == "no_cnpj") 
                                {
                                    continue;
                                }
                                Filtro obj = new Filtro();
                                string value = row[0].ToString();
                                string date_start = row[1].ToString();
                                string date_end = row[2].ToString();
                                string cd_mei = row[3].ToString();

                                obj.NoCep = value;
                                obj.DtInicial = DateTime.Parse(date_start);
                                obj.DtFinal = DateTime.Parse(date_end);
                                obj.CdMei = cd_mei;
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
                var filepath = _configurationManager["DirectoryPath"] + "Atividade (2).xlsx";
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
                var filepath = _configurationManager["DirectoryPath"] + "Cep r02.xlsm";
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
        public async Task<bool> SeedURAWithErrorsData()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            List<UraError> list = new List<UraError>();
            try
            {
                var filepath = _configurationManager["UraFilePath"];
                using (var streamval = File.Open(filepath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = ExcelReaderFactory.CreateReader(streamval))
                    {
                        var configuration = new ExcelDataSetConfiguration
                        {
                            ConfigureDataTable = _ => new ExcelDataTableConfiguration
                            {
                                UseHeaderRow = true
                            }
                        };
                        var dataSet = reader.AsDataSet(configuration);

                        if (dataSet.Tables.Count > 0)
                        {
                            var dataTable = dataSet.Tables[0];
                           
                            foreach (DataRow row in dataTable.Rows)
                            {
                                UraError obj = new UraError();
                                string NoCnpj = row["NoCnpj"].ToString();
                                string Responsavel = row["Responsavel"].ToString();
                                string CdRzsocial = row["CdRzsocial"].ToString();
                                string CdEmail = row["CdEmail"].ToString();
                                string Fone1 = row["Telefones1"].ToString();
                                string Fone2 = GetColumnValue(row, "Telefones2");
                                string Fone3 = GetColumnValue(row, "Telefones3");
                                string Fone4 = GetColumnValue(row, "Telefones4");

                                obj.NoCnpj = Int64.Parse(NoCnpj);
                                obj.CdRzsocial = CdRzsocial;
                                obj.DsSocio = Responsavel;
                                obj.CdEmail = CdEmail;
                                obj.NoFone1 = Fone1;
                                obj.NoFone2 = Fone2;
                                obj.NoFone3 = Fone3;
                                obj.NoFone4 = Fone4;
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
                            //await _.DeleteAllAsync();
                            await _uraService.CreateURAAsync(list);
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
        private static string GetColumnValue(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) ? row[columnName]?.ToString() ?? string.Empty : string.Empty;
        }
    }
}
